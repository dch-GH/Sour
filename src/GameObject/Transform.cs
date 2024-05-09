namespace Sour;

public struct Transform
{
	public Vector3 Position = Vector3.Zero;
	public Quaternion Rotation = Quaternion.Identity;

	public Transform( Vector3 pos, Quaternion rot )
	{
		Position = pos;
		Rotation = rot;
	}

	public Vector3 Forward => Rotation * Axis.Forward;
	public Vector3 Right => Rotation * Axis.Right;
	public Vector3 Up => Rotation * Axis.Up;
	public Matrix4 Matrix => Matrix4.CreateFromQuaternion( Rotation ) * Matrix4.CreateTranslation( Position );
}
