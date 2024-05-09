namespace Sour;
public static class Log
{
	public static void Info( object i )
	{
		Console.WriteLine( i );
	}

	// Just make it obvious we are logging from engine.
	internal static void InfoInternal( object i )
	{
		Console.WriteLine( string.Format( "[ENGINE]: {0}", i ) );
	}
}
