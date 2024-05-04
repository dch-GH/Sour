// using OpenTK.Graphics.OpenGL4;

// namespace Sour;

// public sealed class Framebuffer
// {
// 	public readonly int Handle;

// 	private int _width;
// 	private int _height;
// 	private int _colorTexture;
// 	private int _objectIdBuffer;

// 	public Framebuffer( int width, int height )
// 	{
// 		_width = width;
// 		_height = height;

// 		Handle = GL.GenBuffer();
// 		GL.BindFramebuffer( FramebufferTarget.Framebuffer, Handle );

// 	}

// 	public void BindTexture( Texture tex )
// 	{
// 		GL.FramebufferTexture( FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment1, tex.Handle, 0 );
// 	}

// 	public int ColorAttachment( int num )
// 	{
// 		GL.DrawBuffers( _objectIdFrameBuffer, [DrawBuffersEnum.ColorAttachment1] );
// 	}
// }
