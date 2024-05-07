using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Sour;

public sealed class CameraController : Component
{
	private Vector3 lookAngles;
	private float moveSpeed = 6;
	private float lookSpeed = 3;

	public override void Update()
	{
		base.Update();
		var args = Game.CurrentFrameEvent;

		var dt = Time.Delta;
		var keyboard = Game.Keyboard;

		var delta = Game.Mouse.Delta;

		var fwd = GameObject.Transform.Forward;
		var right = GameObject.Transform.Right;
		var up = GameObject.Transform.Up;

		Vector3 wishDir = Vector3.Zero;

		if ( keyboard.IsKeyDown( Keys.W ) )
			wishDir += fwd;
		if ( keyboard.IsKeyDown( Keys.S ) )
			wishDir -= fwd;

		if ( keyboard.IsKeyDown( Keys.A ) )
			wishDir += right;
		if ( keyboard.IsKeyDown( Keys.D ) )
			wishDir -= right;

		if ( keyboard.IsKeyDown( Keys.R ) )
			wishDir += up;
		if ( keyboard.IsKeyDown( Keys.F ) )
			wishDir -= up;

		if ( wishDir.LengthSquared > 0 )
			wishDir.Normalize();

		wishDir *= moveSpeed * dt;

		if ( keyboard.IsKeyDown( Keys.Left ) )
			lookAngles += Axis.Up * lookSpeed * dt;
		if ( keyboard.IsKeyDown( Keys.Right ) )
			lookAngles -= Axis.Up * lookSpeed * dt;


		if ( !Game.Mouse.Visible )
		{
			if ( keyboard.IsKeyDown( Keys.Down ) )
				lookAngles += Axis.Right * lookSpeed * dt;
			if ( keyboard.IsKeyDown( Keys.Up ) )
				lookAngles += Axis.Left * lookSpeed * dt;

			lookAngles.X += delta.Y * lookSpeed * dt;
			lookAngles.Y -= delta.X * lookSpeed * dt;
		}

		GameObject.Transform.Position += wishDir;
		GameObject.Transform.Rotation = Quaternion.FromAxisAngle( Axis.Up, lookAngles.Y ) * Quaternion.FromAxisAngle( Axis.Right, lookAngles.X );
	}
}
