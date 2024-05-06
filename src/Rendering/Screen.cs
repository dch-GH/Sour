using System.Net.WebSockets;
using OpenTK.Graphics.OpenGL4;

namespace Sour;

public struct Screen
{
	public Vector2 ScreenSize;
	public Material Material;
	private VertexBuffer _vb;
	private int _gameFbo;
	private int _rbo;
	private Texture _colorTexture;
	private Texture _objectIdTexture;
	// public static float[] QuadPositions = [
	// 	-1f, 1f, 0f,
	// 	-1f, -1f, 0f,
	// 	1f, -1f, 0f,

	// 	-1f, 1f, 0f,
	// 	1f, -1f, 0f,
	// 	1f, 1f, 0f];

	// public static float[] QuadUVs = [
	// 	0.0f, 1.0f, 0.0f,
	// 	0.0f, 0.0f, 0.0f,
	// 	1.0f, 0.0f, 0.0f,
	// 	0.0f, 1.0f, 0.0f,
	// 	1.0f, 0.0f, 0.0f,
	// 	1.0f, 1.0f, 0.0f,
	// ];

	public static float[] QuadPositions = [
		-1.0f,  1.0f,  0.0f, 1.0f,
		-1.0f, -1.0f,  0.0f, 0.0f,
		 1.0f, -1.0f,  1.0f, 0.0f,

		-1.0f,  1.0f,  0.0f, 1.0f,
		 1.0f, -1.0f,  1.0f, 0.0f,
		 1.0f,  1.0f,  1.0f, 1.0f
	];

	public float[] Vertices;
	public Texture ColorTexture => _colorTexture;
	public Texture ObjectIdTexture => _objectIdTexture;

	public Screen( Vector2 size, Material material, VertexBuffer vb )
	{
		ScreenSize = size;

		var vertices = new List<float>();
		foreach ( var p in QuadPositions )
			vertices.Add( p );

		// foreach ( var uv in QuadUVs )
		// 	vertices.Add( uv );

		Vertices = vertices.ToArray();

		Material = material;
		_vb = vb;

		CreateFBO();
	}

	private void CreateFBO()
	{
		_colorTexture = new Texture( ScreenSize );
		_objectIdTexture = new Texture( ScreenSize );

		_gameFbo = GL.GenFramebuffer();
		GL.BindFramebuffer( FramebufferTarget.Framebuffer, _gameFbo );
		GL.FramebufferTexture2D( FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, _colorTexture.Handle, 0 );
		GL.FramebufferTexture2D( FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment1, TextureTarget.Texture2D, _objectIdTexture.Handle, 0 );

		_rbo = GL.GenRenderbuffer();
		GL.BindRenderbuffer( RenderbufferTarget.Renderbuffer, _rbo );
		GL.RenderbufferStorage( RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, (int)ScreenSize.X, (int)ScreenSize.Y );
		GL.FramebufferRenderbuffer( FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, _rbo );
		GL.BindFramebuffer( FramebufferTarget.Framebuffer, 0 );

		var error = GL.CheckFramebufferStatus( FramebufferTarget.Framebuffer );
		if ( error is not FramebufferErrorCode.FramebufferComplete )
		{
			Log.Info( $"Failed to create FBO for Screen{error}" );
		}

		GL.BindFramebuffer( FramebufferTarget.Framebuffer, 0 );
	}

	public void PreDraw()
	{
		// Bind to the game FBO.
		GL.BindFramebuffer( FramebufferTarget.Framebuffer, _gameFbo );
		GL.Viewport( 0, 0, (int)ScreenSize.X, (int)ScreenSize.Y );
	}

	public void Draw()
	{
		// Draw the screen quad.
		GL.BindFramebuffer( FramebufferTarget.Framebuffer, 0 );
		GL.Viewport( 0, 0, (int)ScreenSize.X, (int)ScreenSize.Y );

		// Clear the actual view buffer.
		GL.ClearColor( 0.05f, 0.25f, 0.3f, 1 );
		GL.Clear( ClearBufferMask.ColorBufferBit );

		_vb.DrawScreenQuad( this );
	}

	public void Resize( int width, int height )
	{
		ScreenSize = new Vector2( width, height );

		if ( width == 0 || height == 0 )
			ScreenSize = Vector2.One;

		GL.DeleteFramebuffer( _gameFbo );
		CreateFBO();
	}
}
