using OpenTK.Mathematics;

namespace Sour;
public struct Transform
{
	public Vector3 Position;
	public Quaternion Rotation;

	public Transform()
	{
		Position = Vector3.Zero;
		Rotation = Quaternion.Identity;
	}

	public Vector3 Forward => Rotation * Vector3.UnitZ;
	public Vector3 Right => Rotation * Vector3.UnitX;
	public Vector3 Up => Rotation * Vector3.UnitY;
	public Matrix4 Matrix => Matrix4.CreateTranslation( Position ) * Matrix4.CreateFromQuaternion( Rotation );
}
