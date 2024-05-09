global using OpenTK;
global using OpenTK.Graphics;
global using OpenTK.Graphics.GL;
global using OpenTK.Mathematics;
global using OpenTK.Windowing;
global using OpenTK.Windowing.Desktop;

using OpenTK.Windowing.Common;

namespace Sour;

public static class Program
{
	static void Main( string[] args )
	{
		Console.WriteLine( "Running..." );

		var native = NativeWindowSettings.Default;

		native.Vsync = VSyncMode.On;
		native.Title = "Sour Engine - Game";
		native.ClientSize = new Vector2i( 800, 900 );

		var engine = new Engine( GameWindowSettings.Default, native );
		engine.Run();
	}
}
