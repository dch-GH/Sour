using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Sour;

public struct Mouse
{
	public MouseState State;
	public Vector2 Position => Visible ? State.Position : Game.ScreenSize / 2;
	public Vector2 Delta;
	public bool Visible;

}
