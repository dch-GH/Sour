using OpenTK.Windowing.Common;

namespace Sour;

public enum UpdateStage
{
	PreRender,
	PostRender,
	PreUpdate,
	Update,
	PostUpdate
}

public struct UpdateEventEmitter()
{
	public Action<UpdateStage, FrameEventArgs> OnUpdateStage;
}
