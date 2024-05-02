namespace Sour;

public abstract class Component : IDisposable
{
	public GameObject GameObject => _gameObject;
	protected GameObject _gameObject;

	public void Attach( GameObject go )
	{
		_gameObject = go;
	}

	public virtual void Update() { }
	public virtual void Render() { }

	public void Dispose()
	{
	}
}
