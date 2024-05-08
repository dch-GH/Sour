using OpenTK.Windowing.Common;

namespace Sour;

public struct FPSCounter
{
	public int FPS;
	private float _timeStamp;
	private int _frames;

	public FPSCounter()
	{
		Game.UpdateEmitter.OnUpdateStage += Update;
	}

	private void Update( UpdateStage stage, FrameEventArgs args )
	{
		if ( stage is not UpdateStage.PostUpdate )
			return;

		var currentTime = Time.Elapsed;
		_frames += 1;

		// If a second has passed.
		if ( currentTime - _timeStamp >= 1.0 )
		{
			Log.Info( string.Format( "FPS: {0}", _frames ) );
			_frames = 0;
			_timeStamp = currentTime;
		}
	}
}
