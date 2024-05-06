using OpenTK.Windowing.Common;

namespace Sour;

public enum RenderStage
{
	PreEverything,
	PreSceneRender,
	SceneRender,
	PostSceneRender,
	DebugDraw,
}

public sealed class RenderEventEmitter
{
	public Action<RenderStage, FrameEventArgs> OnRenderStage;
}
