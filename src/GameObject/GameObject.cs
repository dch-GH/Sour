using OpenTK.Graphics.OpenGL4;

namespace Sour;

public sealed class GameObject : IDisposable
{
	// Unique Id.
	public uint Id => _id;
	private readonly uint _id;
	public bool Uncullable;
	public static List<GameObject> All => _all;
	public Transform2d Transform;
	public Vector2 Position => Transform.Position;
	public float Rotation => Transform.Rotation;
	public Box3 Bounds;
	public Color4 ColorId => _colorId;
	private Color4 _colorId;
	public bool IsSelected => _selected;
	private bool _selected = false;

	internal static List<GameObject> _all = new();
	private HashSet<Component> _components;

	internal GameObject()
	{
		_id = Util.TimestampId();

		byte red = (byte)(_id >> 16 & 0xFF);
		byte green = (byte)(_id >> 8 & 0xFF);
		byte blue = (byte)(_id & 0xFF);


		var rf = red.ByteToFloat();
		var gf = green.ByteToFloat();
		var bf = blue.ByteToFloat();
		_colorId = new Color4( rf, gf, bf, 1.0f );

		if ( Engine.DebugObjectIdMousePick )
		{
			Log.InfoInternal( $"Color in bytes: R: {red}, G: {green}, B: {blue}" );
			Log.InfoInternal( $"Color in floats: R: {rf}, G: {gf}, B: {bf}" );
		}

		Transform = new();
		_components = new();

		// TODO:
		Bounds = new Box3( Vector3.One * -1.2f, Vector3.One * 1.2f );
		_all.Add( this );
	}

	~GameObject()
	{
		Dispose();
	}

	public void Delete()
	{
		_all.Remove( this );
		Dispose();
	}

	public T AddComponent<T>( T component ) where T : Component
	{
		_components.Add( component );
		component.Attach( this );
		return component;
	}

	public T AddComponent<T>() where T : Component, new()
	{
		var component = new T();
		_components.Add( component );
		component.Attach( this );
		return component;
	}

	public void Update()
	{
		foreach ( Component? c in _components )
		{
			c.Update();
		}
	}

	public void Render()
	{
		foreach ( Component? c in _components )
		{
			c.Render();
		}
	}

	public static void UpdateAll()
	{
		for ( int i = 0; i < _all.Count; i++ )
		{
			GameObject? go = _all[i];
			go.Update();
		}
	}

	public static void RenderAll()
	{
		for ( int i = 0; i < _all.Count; i++ )
		{
			GameObject? go = _all[i];
			if ( !go.Uncullable && !Camera.Main.ShouldRender( go.Transform.Position.ToVector3() ) )
				continue;
			go.Render();
		}
	}

	public static GameObject Spawn( Vector2 position, float Rotation )
	{
		var go = new GameObject();
		go.Transform.Position = position;
		go.Transform.Rotation = Rotation;
		return go;
	}

	public static GameObject Spawn()
	{
		return Spawn( Vector2.Zero, 0.0f );
	}

	public static GameObject? GetFromId( Color255 color )
	{
		var col4 = color.ToColor4();
		// foreach ( var go in All )
		// {
		// 	Log.InfoInternal( $"Looking for {col4}" );
		// 	Log.InfoInternal( $"This GameObject has {go.ColorId}" );
		// }
		return All.FirstOrDefault( x => x.ColorId == col4 );
	}

	public static GameObject? TryGetAt( Vector2i position )
	{
		var x = position.X;
		var y = position.Y;

		GL.ReadBuffer( ReadBufferMode.ColorAttachment1 );

		Color255 pixelData = new();
		GL.ReadPixels( x, y, 1, 1, PixelFormat.Rgba, PixelType.UnsignedByte, ref pixelData );

		if ( Engine.DebugObjectIdMousePick )
		{
			Log.InfoInternal( $"Pixel color (bytes): {pixelData}" );
			Log.InfoInternal( $"Pixel color (floats): {pixelData.ToColor4()}" );
		}

		return GetFromId( pixelData );
	}

	public static GameObject? TryGetAt( Vector2 position )
	{
		return TryGetAt( new Vector2i( (int)position.X, (int)position.Y ) );
	}

	public void ToggleSelected()
	{
		_selected = !_selected;
		if ( _selected )
		{
			Log.InfoInternal( $"GameObject : {_id} is now selected." );
		}
		else
		{
			Log.InfoInternal( $"GameObject : {_id} is now deselected." );
		}
	}

	public void Dispose()
	{
		foreach ( var component in _components )
		{
			_components.Remove( component );
		}

		_components.Clear();
	}
}
