using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Sour;

public sealed class CameraController : Component
{
	private Quaternion lookAngles;
	private float moveSpeed = 6;
	private float lookSpeed = 64;

	private float _pitch;
	private float _yaw;
	private CircularBuffer<Vector2> _deltaBuffer = new( 2000 );
	private bool _recordingDelta;
	private float _timeSinceRecordingStarted;


	public override void Render()
	{
		var dt = Time.Delta;
		var keyboard = Engine.Keyboard;
		var delta = Engine.Mouse.Delta;

		if ( _timeSinceRecordingStarted >= 1.0f )
		{
			if ( _recordingDelta )
			{
				Log.Info( _deltaBuffer.ToString() );
			}
			_recordingDelta = false;
			_timeSinceRecordingStarted = 0;
		}

		if ( keyboard.IsKeyReleased( Keys.G ) && _timeSinceRecordingStarted <= 0.0f )
		{
			_recordingDelta = true;
			_timeSinceRecordingStarted = 0;
			_deltaBuffer.Clear();
		}

		if ( _recordingDelta )
		{
			_deltaBuffer.Add( delta );
			_timeSinceRecordingStarted += Time.Delta;
		}

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
			_yaw += lookSpeed * 0.2f * dt;
		if ( keyboard.IsKeyDown( Keys.Right ) )
			_yaw -= lookSpeed * 0.2f * dt;

		if ( keyboard.IsKeyDown( Keys.Down ) )
			_pitch += lookSpeed * 0.2f * dt;
		if ( keyboard.IsKeyDown( Keys.Up ) )
			_pitch -= lookSpeed * 0.2f * dt;

		if ( !Engine.Mouse.Visible )
		{
			_pitch += delta.Y * lookSpeed * dt;
			_yaw -= delta.X * lookSpeed * dt;
		}

		var pitchDegrees = MathHelper.RadiansToDegrees( _pitch );
		_pitch = MathHelper.DegreesToRadians( Math.Clamp( pitchDegrees, -89.0f, 89.0f ) );

		GameObject.Transform.Position += wishDir;
		lookAngles = Quaternion.FromAxisAngle( Axis.Up, _yaw ) * Quaternion.FromAxisAngle( Axis.Right, _pitch );
		GameObject.Transform.Rotation = lookAngles;
	}
}
