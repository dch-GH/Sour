namespace Sour;

public sealed class GameObject : IDisposable
{
	// Unique Id.
	public uint Id => Id;
	public static List<GameObject> All => _all;
	public Transform Transform;
	public Box3 Bounds;
	private readonly uint _id;

	// How many times a GameObject has been created.
	// Use this for assigning a unique ID to every GameObject.
	private static uint _generation;
	private static List<GameObject> _all = new();
	private List<Component> _components;

	private GameObject()
	{
		_id = _generation;
		_generation += 1;

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
