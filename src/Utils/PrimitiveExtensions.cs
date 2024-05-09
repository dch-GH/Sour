namespace Sour;

public static class PrimitiveExtensions
{
	public static int RgbToHex( this int self )
	{
		int red = (self >> 16) & 0xFF;
		int green = (self >> 8) & 0xFF;
		int blue = self & 0xFF;

		return (red << 16) | (green << 8) | blue;
	}

	public static float ByteToFloat( this byte self )
	{
		return self / 255.0f;
	}

	public static byte FloatToByte( this float self )
	{
		return (byte)Math.Round( self * 255.0f );
	}
}
