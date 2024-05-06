public struct Color255
{
	public byte R;
	public byte G;
	public byte B;
	public byte A;

	public static Color255 Transparent => new Color255( 0, 0, 0, 0 );
	public static Color255 White => new Color255( 255, 255, 255, 255 );
	public static Color255 Black => new Color255( 0, 0, 0, 255 );

	public Color255( byte r, byte g, byte b, byte a )
	{
		R = r;
		G = g;
		B = b;
		A = a;
	}

	public Color255( uint color )
	{
		R = (byte)(color >> 16 & 0xFF);
		G = (byte)(color >> 8 & 0xFF);
		B = (byte)(color & 0xFF);
	}

	public Color4 ToColor4()
	{
		const float max = 255.0f;
		return new Color4( R / max, G / max, B / max, 1.0f );
	}

	public override string ToString()
	{
		return $"R: {R}, G: {G}, B: {B}, A: {A}";
	}
}
