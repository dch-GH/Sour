using System.Net.WebSockets;
using OpenTK.Graphics.OpenGL4;

namespace Sour;

public struct Screen
{
	public Vector2i Size;

	public Material Material;
	private VertexBuffer _vb;
	private int _gameFbo;
	private int _rbo;
	private Texture _colorTexture;
	private Texture _objectIdTexture;
	public static float[] QuadVertsUvs = [
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
	private bool _isResizing;
	private bool _wasResizing;

	public Screen( Vector2i size, Material material, VertexBuffer vb )
	{
		Size = size;

		var vertices = new List<float>();
		foreach ( var p in QuadVertsUvs )
			vertices.Add( p );

		Vertices = vertices.ToArray();

		Material = material;
		_vb = vb;

		CreateFBO();
	}

	private void CreateFBO()
	{
		_colorTexture = new Texture( Size );
		_objectIdTexture = new Texture( Size );

		_gameFbo = GL.GenFramebuffer();
		GL.BindFramebuffer( FramebufferTarget.Framebuffer, _gameFbo );
		GL.FramebufferTexture2D( FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, _colorTexture.Handle, 0 );
		GL.FramebufferTexture2D( FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment1, TextureTarget.Texture2D, _objectIdTexture.Handle, 0 );

		_rbo = GL.GenRenderbuffer();
		GL.BindRenderbuffer( RenderbufferTarget.Renderbuffer, _rbo );
		GL.RenderbufferStorage( RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, (int)Size.X, (int)Size.Y );
		GL.FramebufferRenderbuffer( FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, _rbo );

		var error = GL.CheckFramebufferStatus( FramebufferTarget.Framebuffer );
		if ( error is not FramebufferErrorCode.FramebufferComplete )
		{
			Log.Info( $"Failed to create FBO for Screen {error}" );
		}

		GL.BindFramebuffer( FramebufferTarget.Framebuffer, 0 );
	}

	public void PreDraw()
	{
		if ( !_isResizing && _wasResizing )
			OnResizeFinished( Size );

		// Bind to the game FBO.
		GL.BindFramebuffer( FramebufferTarget.Framebuffer, _gameFbo );
		GL.Viewport( 0, 0, Size.X, Size.Y );
	}

	public void Draw()
	{
		// Draw the screen quad.
		GL.BindFramebuffer( FramebufferTarget.Framebuffer, 0 );
		GL.Viewport( 0, 0, Size.X, Size.Y );

		// Clear the actual view buffer.
		GL.ClearColor( 0.05f, 0.25f, 0.3f, 1 );
		GL.Clear( ClearBufferMask.ColorBufferBit );

		_vb.DrawScreenQuad( this );
		_wasResizing = _isResizing;
		_isResizing = false;
	}

	public void Resize( Vector2i nextSize )
	{
		Size = nextSize;
		_isResizing = true;
	}

	public void OnResizeFinished( Vector2i size )
	{
		Size = size;

		if ( size.X == 0 || size.Y == 0 )
			Size = Vector2i.One;

		GL.DeleteFramebuffer( _gameFbo );
		CreateFBO();
	}
}
