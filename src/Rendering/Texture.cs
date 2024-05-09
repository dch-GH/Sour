using OpenTK.Graphics.OpenGL4;

namespace Sour;

public enum TextureFilterMode
{
	Nearest,
	Linear,
}

public sealed class Texture
{
	public readonly int Handle;

	public Texture( int width, int height, TextureFilterMode filterMode = TextureFilterMode.Nearest )
	{
		Handle = GL.GenTexture();
		GL.BindTexture( TextureTarget.Texture2D, Handle );
		GL.TexImage2D( TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba32f, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, 0 );


		if ( filterMode == TextureFilterMode.Nearest )
		{
			GL.TexParameterI( TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, new int[] { (int)TextureMinFilter.Nearest } );
			GL.TexParameterI( TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, new int[] { (int)TextureMagFilter.Nearest } );
		}
		else if ( filterMode == TextureFilterMode.Linear )
		{
			GL.TexParameterI( TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, new int[] { (int)TextureMinFilter.Linear } );
			GL.TexParameterI( TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, new int[] { (int)TextureMagFilter.Linear } );
		}

		GL.TexParameterI( TextureTarget.Texture2D, TextureParameterName.TextureWrapS, new int[] { (int)TextureWrapMode.ClampToBorder } );
		GL.TexParameterI( TextureTarget.Texture2D, TextureParameterName.TextureWrapT, new int[] { (int)TextureWrapMode.ClampToBorder } );
		GL.BindTexture( TextureTarget.Texture2D, 0 );
	}

	public Texture( Vector2i size, TextureFilterMode filterMode = TextureFilterMode.Nearest ) : this( size.X, size.Y, filterMode ) { }

	public void Resize( int width, int height )
	{
		GL.BindTexture( TextureTarget.Texture2D, Handle );
		GL.TexImage2D( TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba32f, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, 0 );
		GL.BindTexture( TextureTarget.Texture2D, 0 );
	}
}
