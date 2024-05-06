using OpenTK.Windowing.Common;

namespace Sour;

public enum UpdateStage
{
	PreRender,
	PostRender,
}

public struct UpdateEventEmitter()
{
	public Action<UpdateStage, FrameEventArgs> OnUpdateStage;
}
