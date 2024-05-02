namespace Sour;

class RotatorComponent : Component
{
	public override void Update()
	{
		base.Update();
		GameObject.Transform.Rotation = Quaternion.FromAxisAngle( Vector3.UnitY + Vector3.UnitZ * 0.5f, Time.Elapsed );
	}
}
