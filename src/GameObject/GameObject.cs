using OpenTK.Graphics.OpenGL4;

namespace Sour;

public sealed class GameObject : IDisposable
{
	// Unique Id.
	public uint Id => _id;
	private readonly uint _id;
	public static List<GameObject> All => _all;
	public Transform Transform;
	public Box3 Bounds;
	public Color4 ColorId => _colorId;
	private Color4 _colorId;
	public bool IsSelected => _selected;
	private bool _selected = false;

	private static List<GameObject> _all = new();
	private List<Component> _components;

	private GameObject()
	{
		DateTime epochStart = new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );
		TimeSpan span = DateTime.UtcNow - epochStart;

		_id = (uint)span.TotalMilliseconds;

		byte red = (byte)(_id >> 16 & 0xFF);
		byte green = (byte)(_id >> 8 & 0xFF);
		byte blue = (byte)(_id & 0xFF);

		Log.Info( $"Color in bytes: R: {red}, G: {green}, B: {blue}" );

		var rf = red.ByteToFloat();
		var gf = green.ByteToFloat();
		var bf = blue.ByteToFloat();
		Log.Info( $"Color in floats: R: {rf}, G: {gf}, B: {bf}" );

		_colorId = new Color4( rf, gf, bf, 1.0f );

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

	public void AddComponent<T>( T component ) where T : Component
	{
		_components.Add( component );
		component.Attach( this );
	}

	public void AddComponent<T>() where T : Component, new()
	{
		var t = new T();
		_components.Add( t );
		t.Attach( this );
	}

	public void Update()
	{
		for ( int i = 0; i < _components.Count; i++ )
		{
			Component? c = _components[i];
			c.Update();
		}
	}

	public void Render()
	{
		for ( int i = 0; i < _components.Count; i++ )
		{
			Component? c = _components[i];
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
			go.Render();
		}
	}

	public static GameObject Spawn( Vector3 position, Quaternion Rotation )
	{
		var go = new GameObject();
		go.Transform.Position = position;
		go.Transform.Rotation = Rotation;
		return go;
	}

	public static GameObject Spawn()
	{
		return Spawn( Vector3.Zero, Quaternion.Identity );
	}

	public static GameObject? GetFromId( Color255 color )
	{
		var col4 = color.ToColor4();
		// foreach ( var go in All )
		// {
		// 	Log.Info( $"Looking for {col4}" );
		// 	Log.Info( $"This GameObject has {go.ColorId}" );
		// }
		return All.FirstOrDefault( x => x.ColorId == col4 );
	}

	public static GameObject? TryGetAt( Vector2i position )
	{
		var mouseX = position.X;
		var mouseY = position.Y;

		GL.ReadBuffer( ReadBufferMode.ColorAttachment1 );

		Color255 pixelData = new();
		GL.ReadPixels( mouseX, mouseY, 1, 1, PixelFormat.Rgba, PixelType.UnsignedByte, ref pixelData );

		// Log.Info( $"Pixel color (bytes): {pixelData}" );
		// var red = pixelData.R;
		// var green = pixelData.G;
		// var blue = pixelData.B;
		// Log.Info( $"Pixel color (floats): R: {red / 255f}, G: {green / 255f}, B: {blue / 255f}" );

		return GetFromId( pixelData );
	}

	public static GameObject? TryGetAt( Vector2 position )
	{
		return TryGetAt( new Vector2i( (int)position.X, (int)position.Y ) );
	}

	public void ToggleSelected()
	{
		_selected = !_selected;
		// if ( _selected )
		// {
		// 	Log.Info( $"GameObject : {_id} is now selected." );
		// }
		// else
		// {
		// 	Log.Info( $"GameObject : {_id} is now deselected." );
		// }
	}

	public void Dispose()
	{
		foreach ( var component in _components )
		{
			component.Dispose();
			_components.Remove( component );
		}

		_components.Clear();
	}
}
