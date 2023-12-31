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
		Console.WriteLine( "Hello, World!" );
		var native = NativeWindowSettings.Default;
		native.ClientSize = new Vector2i( 800, 900 );
		var window = new SourWindow( GameWindowSettings.Default, native );

		window.Run();
	}
}
