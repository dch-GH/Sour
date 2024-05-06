namespace Sour;

public sealed class GameObject : IDisposable
{
	// Unique Id.
	public uint Id => _id;
	public static List<GameObject> All => _all;
	public Transform Transform;
	public Box3 Bounds;
	private readonly uint _id;

	// How many times a GameObject has been created.
	// Use this for assigning a unique ID to every GameObject.
	private static uint _generation;
	private static List<GameObject> _all = new();
	private List<Component> _components;

	// TODO: hack
	public Color4 ColorId => _colorId;
	private Color4 _colorId;

	private GameObject()
	{
		DateTime epochStart = new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );
		TimeSpan span = DateTime.UtcNow - epochStart;

		_id = (uint)span.TotalMilliseconds;

		byte red = (byte)(_id >> 16 & 0xFF);
		byte green = (byte)(_id >> 8 & 0xFF);
		byte blue = (byte)(_id & 0xFF);

		Log.Info( $"Color in bytes: R: {red}, G: {green}, B: {blue}" );

		var rf = Game.ByteToFloat( red );
		var gf = Game.ByteToFloat( green );
		var bf = Game.ByteToFloat( blue );
		Log.Info( $"Color in floats: R: {rf}, G: {gf}, B: {bf}" );

		_colorId = new Color4( rf, gf, bf, 1.0f );

		Transform = new();
		_components = new();

		// TODO:
		Bounds = new Box3( Vector3.One * -1, Vector3.One * 1 );
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
