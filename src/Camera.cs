using System.Drawing;
using OpenTK.Windowing.Common;

namespace Sour;
public class Camera
{
	public static Camera Main;
	public Transform Transform;
	public float FieldOfView = 90;

	public Matrix4 ViewMatrix;
	public Matrix4 ProjectionMatrix;

	public Camera( Vector3 position, Quaternion rotation, float fov = 90 )
	{
		Transform = new( position, rotation );
		FieldOfView = fov;
		ViewMatrix = Matrix4.LookAt( Transform.Position, Vector3.Zero, Vector3.UnitY );
		ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(
			MathHelper.DegreesToRadians( FieldOfView ),
			Game.ScreenSize.X / (float)Game.ScreenSize.Y,
			0.1f,
			300.0f
		);
	}

	public void Update( FrameEventArgs args )
	{
		ViewMatrix = Matrix4.LookAt(
			Transform.Position,
			Transform.Position + Transform.Forward,
			Vector3.UnitY
		);

		ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(
			MathHelper.DegreesToRadians( FieldOfView ),
			Game.ScreenSize.X / (float)Game.ScreenSize.Y,
			0.1f,
			300.0f
		);

		DebugDraw.Line( Transform.Position - Vector3.UnitY * 2, Transform.Position + Transform.Forward * 200, Color.White );
	}
}
