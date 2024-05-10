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

		Vector2 wishDir = Vector2.Zero;

		if ( keyboard.IsKeyDown( Keys.W ) )
			wishDir += Axis2d.Up;
		if ( keyboard.IsKeyDown( Keys.S ) )
			wishDir -= Axis2d.Up;

		if ( keyboard.IsKeyDown( Keys.A ) )
			wishDir += Axis2d.Right;
		if ( keyboard.IsKeyDown( Keys.D ) )
			wishDir -= Axis2d.Right;

		if ( wishDir.LengthSquared > 0 )
			wishDir.Normalize();

		wishDir *= moveSpeed * dt;
		GameObject.Transform.Position += wishDir;
	}
}
