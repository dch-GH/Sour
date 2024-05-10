namespace Sour;

class RotatorComponent : Component
{
	public override void Update()
	{
		base.Update();
		GameObject.Transform.Rotation += Time.Delta;
	}
}
