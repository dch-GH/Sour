using OpenTK.Compute.OpenCL;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Sour;

public struct Screen
{
	public Material Material;
	private VertexBuffer _vb;
	public static float[] QuadPositions = [-1f, 1f, 0f, -1f, -1f, 0f, 1f, -1f, 0f, -1f, 1f, 0f, 1f, -1f, 0f, 1f, 1f, 0f];
	public static float[] QuadUVs = [
		0.0f, 1.0f, 0.0f,
		0.0f, 0.0f, 0.0f,
		1.0f, 0.0f, 0.0f,
		0.0f, 1.0f, 0.0f,
		1.0f, 0.0f, 0.0f,
		1.0f, 1.0f, 0.0f,
	];

	public float[] Vertices;

	private Texture _test;

	public Screen( Material material, VertexBuffer vb, Texture test )
	{
		var vertices = new List<float>();
		foreach ( var p in QuadPositions )
		{
			vertices.Add( p );
		}

		foreach ( var uv in QuadUVs )
		{
			vertices.Add( uv );
		}

		Vertices = vertices.ToArray();

		Material = material;
		_vb = vb;
		_test = test;
	}

	public void Draw()
	{
		if ( Material is null )
		{
			throw new Exception( "ow" );
		}

		_vb.DrawScreenQuad( this, _test );
	}
}

public sealed class Game : GameWindow
{
	// TODO: Make this a Vector2i.
	public static Vector2 ScreenSize;
	public static KeyboardState Keyboard;
	public static MouseState Mouse;
	public static MaterialManager Materials;
	public static ModelRenderer ModelRenderer;

	Camera MainCamera;

	Vector3 lookAngles;
	float moveSpeed = 6;
	float lookSpeed = 3;

	private int _gameFbo;
	private int _rbo;
	private Texture _colorTexture;
	private Texture _objectIdTexture;
	private Screen _screen;

	public Game( GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings ) : base( gameWindowSettings, nativeWindowSettings )
	{
	}

	protected override void OnLoad()
	{
		base.OnLoad();

		ScreenSize = ClientSize;

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
			Log.Info( $"onload {error}" );
			// Close();
		}

		MainCamera = new Camera( new Vector3( 0, 0, -5 ), Quaternion.Identity );
		Camera.Main = MainCamera;

		Materials = new();
		DebugDraw.Init();

		ModelRenderer = new( this, MainCamera );

		var screenMaterial = new Material( "Resources/Shaders/Screen/vert_screen.glsl", "Resources/Shaders/Screen/frag_screen.glsl" );
		_screen = new( screenMaterial, new VertexBuffer(), _colorTexture );

		var cube = GameObject.Spawn();
		cube.AddComponent<ModelComponent>( new ModelComponent( "Resources/Models/Box/box.fbx" ) );

		var cone = GameObject.Spawn();
		cone.AddComponent( new ModelComponent( "Resources/Models/Cone/cone.obj", new Material( fragShaderPath: "Resources/Shaders/frag.glsl" ) ) );
		cone.Transform.Position += Vector3.UnitX * 3f;
		cone.AddComponent<RotatorComponent>();
	}

	public override void Run()
	{
		base.Run();
	}

	protected override void OnRenderFrame( FrameEventArgs args )
	{
		base.OnRenderFrame( args );

		// Update materials first for a chance to hotload shaders.
		if ( Materials.AnyShadersNeedHotload )
		{
			Materials.TryHotloadShaders();
		}

		MainCamera.Update( args );

		// GameObjects and Components submit drawtasks here, which are drawn later in this loop.
		GameObject.RenderAll();

		{
			// Bind to the game FBO.
			GL.BindFramebuffer( FramebufferTarget.Framebuffer, _gameFbo );
			GL.Viewport( 0, 0, (int)ScreenSize.X, (int)ScreenSize.Y );
			// GL.DrawBuffers( _gameFbo, [DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1] );
			// GL.DrawBuffer( DrawBufferMode.ColorAttachment0 );

			// Clear the scene.
			GL.ClearColor( 0.1f, 0f, 0.2f, 1f );
			GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

			GL.Enable( EnableCap.CullFace );
			GL.Enable( EnableCap.DepthTest );

			// Draw the scene.
			GameObject.RenderAll();
			ModelRenderer.Render( args );

			// Draw the screen quad.
			GL.BindFramebuffer( FramebufferTarget.Framebuffer, 0 );
			GL.Viewport( 0, 0, (int)ScreenSize.X, (int)ScreenSize.Y );

			// Clear the actual view buffer.
			GL.ClearColor( 0.05f, 0.25f, 0.3f, 1 );
			GL.Clear( ClearBufferMask.ColorBufferBit );
			_screen.Draw();

			SwapBuffers();
		}

		Time.Delta = (float)args.Time;
		Time.Elapsed += Time.Delta;
	}

	protected override void OnResize( ResizeEventArgs e )
	{
		base.OnResize( e );
		GL.Viewport( 0, 0, e.Width, e.Height );
		ScreenSize = ClientSize;
	}

	protected override void OnUpdateFrame( FrameEventArgs args )
	{
		base.OnUpdateFrame( args );
		var dt = ((float)args.Time);
		var keyboard = KeyboardState;
		var mouse = MouseState;

		Keyboard = keyboard;
		Mouse = MouseState;

		// TODO:
		if ( mouse.IsButtonReleased( MouseButton.Left ) )
		{
			// var pos = MainCamera.Transform.Position;
			// var fwd = MainCamera.Transform.Forward;

			// var ray = new Ray( pos, pos + fwd * 10000 );

			// foreach ( var go in GameObject.All )
			// {
			// 	var box3 = go.Bounds.Translated( go.Transform.Position ).Scaled( Vector3.One * 4, go.Transform.Position );
			// 	Log.Info( go.Bounds );
			// 	Log.Info( mouse.Position );
			// 	var box2d = go.Bounds.ToBox2();
			// 	Log.Info( box2d );
			// 	if ( box2d.ContainsExclusive( mouse.Position ) )
			// 	{
			// 		Log.Info( "yes!" );
			// 	}
			// }
		}


		GameObject.UpdateAll();
		ModelRenderer.Update( args, keyboard );

		Vector3 wishDir = Vector3.Zero;

		if ( keyboard.IsKeyDown( Keys.W ) )
			wishDir += MainCamera.Transform.Forward;
		if ( keyboard.IsKeyDown( Keys.S ) )
			wishDir -= MainCamera.Transform.Forward;

		if ( keyboard.IsKeyDown( Keys.A ) )
			wishDir += MainCamera.Transform.Right;
		if ( keyboard.IsKeyDown( Keys.D ) )
			wishDir -= MainCamera.Transform.Right;

		if ( wishDir.LengthSquared > 0 )
			wishDir.Normalize();

		wishDir *= moveSpeed * dt;

		if ( keyboard.IsKeyDown( Keys.Left ) )
			lookAngles += Vector3.UnitY * lookSpeed * dt;
		if ( keyboard.IsKeyDown( Keys.Right ) )
			lookAngles -= Vector3.UnitY * lookSpeed * dt;

		// monkey.Transform.Rotation = Quaternion.FromAxisAngle( Vector3.UnitY, Time.Elapsed );
		// cone.Transform.Rotation = Quaternion.FromAxisAngle( Vector3.UnitX, Time.Elapsed * 2 );

		MainCamera.Transform.Position += wishDir;
		MainCamera.Transform.Rotation = Quaternion.FromEulerAngles( lookAngles );

	}
}
