namespace Sour;

public static class Util
{
	public static TimeSpan TimestampNow()
	{
		DateTime epochStart = new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );
		TimeSpan span = DateTime.UtcNow - epochStart;
		return span;
	}

	public static uint TimestampId()
	{
		DateTime epochStart = new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );
		TimeSpan span = DateTime.UtcNow - epochStart;
		return (uint)span.TotalMilliseconds;
	}
}
