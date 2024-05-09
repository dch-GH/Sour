namespace Sour;

public static class Time
{
	public static float Elapsed;
	public static float Delta;
	public static float DeltaUnscaled;
	public static int Frames;
	public static bool IsFirstFrame => Frames == 0;
}
