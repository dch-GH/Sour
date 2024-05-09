namespace Sour;

class RotatorComponent : Component
{
	public override void Update()
	{
		base.Update();
		GameObject.Transform.Rotation = Quaternion.FromAxisAngle( Axis.Up + Axis.Forward * 0.5f, Time.Elapsed );
	}
}
