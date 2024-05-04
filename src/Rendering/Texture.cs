using System.Text.Encodings.Web;
using OpenTK.Graphics.OpenGL4;

namespace Sour;

public sealed class Texture
{
	public readonly int Handle;

	public Texture( int width, int height )
	{
		Handle = GL.GenTexture();
		GL.BindTexture( TextureTarget.Texture2D, Handle );
		GL.TexImage2D( TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16f, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, 0 );
		GL.TexParameterI( TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, new int[] { (int)TextureMinFilter.Nearest } );
		GL.TexParameterI( TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, new int[] { (int)TextureMagFilter.Nearest } );
		GL.BindTexture( TextureTarget.Texture2D, 0 );
	}

	public Texture( Vector2 size ) : this( (int)size.X, (int)size.Y ) { }
}
