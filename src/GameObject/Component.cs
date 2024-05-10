namespace Sour;

public abstract class Component
{
	public GameObject GameObject => _gameObject;
	protected GameObject _gameObject;
	public Transform2d Transform => _gameObject.Transform;
	public Vector2 Position => _gameObject.Position;
	public float Rotation => _gameObject.Rotation;

	internal void Attach( GameObject go )
	{
		if ( go is null )
			throw new Exception( string.Format( "Null GameObject attempting to attach to Component: {0}", this ) );

		_gameObject = go;
		OnAttached();
	}

	protected virtual void OnAttached() { }

	public virtual void Update() { }
	public virtual void PhysicsUpdate() { }
	public virtual void Render() { }
}
