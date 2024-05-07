namespace Sour;

public abstract class Component : IDisposable
{
	public GameObject GameObject => _gameObject;
	protected GameObject _gameObject;
	public Transform Transform => _gameObject.Transform;

	public void Attach( GameObject go )
	{
		_gameObject = go;
		OnAttached();
	}

	protected virtual void OnAttached() { }

	public virtual void Update() { }
	public virtual void Render() { }

	public void Dispose()
	{
	}
}
