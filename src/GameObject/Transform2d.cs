namespace Sour;

public struct Transform2d
{
	public Vector2 Position = Vector2.Zero;
	public float Rotation = 0.0f;

	public Transform2d( Vector2 pos, float rot )
	{
		Position = pos;
		Rotation = rot;
	}

	public Vector3 Forward => Axis.Forward;
	public Vector3 Right => Axis.Right;
	public Vector3 Up => Axis.Up;
	public Matrix4 Matrix => Matrix4.CreateFromQuaternion( Quaternion.Identity * Quaternion.FromAxisAngle( Vector3.UnitZ, Rotation ) ) * Matrix4.CreateTranslation( new Vector3( Position.X, Position.Y, 0 ) );
}
