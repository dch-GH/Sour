global using OpenTK;
global using OpenTK.Windowing;
global using OpenTK.Graphics;
global using OpenTK.Graphics.GL;
global using OpenTK.Windowing.Desktop;

namespace Sour;

internal class Program
{
	static void Main( string[] args )
	{
		Console.WriteLine( "Hello, World!" );
		var window = new SourWindow( GameWindowSettings.Default, NativeWindowSettings.Default );

		window.Run();

	}
}
