namespace Sour;

public abstract class Component
{
	public GameObject GameObject => _gameObject;
	protected GameObject _gameObject;
	public Transform Transform => _gameObject.Transform;
	public Vector3 Position => _gameObject.Position;
	public Quaternion Rotation => _gameObject.Rotation;

	internal void Attach( GameObject go )
	{
		if ( go is null )
			throw new Exception( string.Format( "Null GameObject attempting to attach to Component: {0}", this ) );

		_gameObject = go;
		OnAttached();
	}

	protected virtual void OnAttached() { }

	public virtual void Update() { }
	public virtual void Render() { }
}
