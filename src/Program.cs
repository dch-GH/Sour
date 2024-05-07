global using OpenTK;
global using OpenTK.Graphics;
global using OpenTK.Graphics.GL;
global using OpenTK.Mathematics;
global using OpenTK.Windowing;
global using OpenTK.Windowing.Desktop;

namespace Sour;

public class Program
{
	static void Main( string[] args )
	{
		Console.WriteLine( "Running..." );
		var native = NativeWindowSettings.Default;
		native.Title = "Sour Engine";
		native.ClientSize = new Vector2i( 800, 900 );
		var window = new Game( GameWindowSettings.Default, native );

		window.Run();
	}
}
