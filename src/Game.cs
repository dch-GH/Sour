using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Sour;

public sealed class Game : GameWindow
{
	// TODO: Make this a Vector2i.
	public static Vector2 ScreenSize;
	private Screen _screen;
	public static KeyboardState Keyboard;
	public static MouseState Mouse;
	public static MaterialManager Materials;
	public static ModelRenderer ModelRenderer;

	Camera MainCamera;

	Vector3 lookAngles;
	float moveSpeed = 6;
	float lookSpeed = 3;
	private Color4 _selectedGameObjectId;

	public Game( GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings ) : base( gameWindowSettings, nativeWindowSettings )
	{
	}

	protected override void OnLoad()
	{
		base.OnLoad();

		ScreenSize = ClientSize;
		Materials = new();

		MainCamera = new Camera( new Vector3( 0, 0, -5 ), Quaternion.Identity );
		Camera.Main = MainCamera;
		DebugDraw.Init();
		ModelRenderer = new( this, MainCamera );

		var screenMaterial = new Material( "Resources/Shaders/Screen/vert_screen.glsl", "Resources/Shaders/Screen/frag_screen.glsl" );
		_screen = new( ScreenSize, screenMaterial, new VertexBuffer() );

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
		var mouse = MouseState;

		// Update materials first for a chance to hotload shaders.
		if ( Materials.AnyShadersNeedHotload )
		{
			Materials.TryHotloadShaders();
		}

		MainCamera.Update( args );

		// GameObjects and Components submit drawtasks here, which are drawn later in this loop.
		GameObject.RenderAll();
		{
			_screen.PreDraw();

			GL.DrawBuffers( 2, [DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1] );

			// Clear the scene.
			GL.ClearColor( 0.1f, 0f, 0.2f, 1f );
			GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
			GL.Enable( EnableCap.CullFace );
			GL.Enable( EnableCap.DepthTest );

			// Draw the scene.
			ModelRenderer.Render( args );

			// TODO:
			if ( mouse.IsButtonReleased( MouseButton.Left ) )
			{
				var mouseX = (int)Mouse.Position.X;
				var mouseY = (int)Mouse.Position.Y;
				GL.ReadBuffer( ReadBufferMode.ColorAttachment1 );

				int pixelData = 0;
				GL.ReadPixels( mouseX, mouseY, 1, 1, PixelFormat.Rgba, PixelType.UnsignedByte, ref pixelData );

				Log.Info( $"pixelData: {pixelData}" );
				// byte red = Color.FromArgb( pixelData ).R;
				// byte green = Color.FromArgb( pixelData ).G;
				// byte blue = Color.FromArgb( pixelData ).B;

				float red = (byte)(pixelData >> 16 & 0xFF) / 255f;
				float green = (byte)(pixelData >> 8 & 0xFF) / 255f;
				float blue = (byte)(pixelData & 0xFF) / 255f;

				Log.Info( $"Color in bytes: R: {red}, G: {green}, B: {blue}" );
			}

			GL.DrawBuffers( 1, [DrawBuffersEnum.ColorAttachment0] );
			GL.Clear( ClearBufferMask.None );

			DebugDraw.Render( args );

			_screen.Draw();

			SwapBuffers();
		}

		Time.Delta = (float)args.Time;
		Time.Elapsed += Time.Delta;
	}

	protected override void OnResize( ResizeEventArgs e )
	{
		base.OnResize( e );
		ScreenSize = ClientSize;
		GL.Viewport( 0, 0, e.Width, e.Height );
		_screen.Resize( e.Width, e.Height );
	}

	// protected override void OnFramebufferResize( FramebufferResizeEventArgs e )
	// {
	// 	base.OnFramebufferResize( e );
	// }

	protected override void OnUpdateFrame( FrameEventArgs args )
	{
		base.OnUpdateFrame( args );
		var dt = ((float)args.Time);
		var keyboard = KeyboardState;
		var mouse = MouseState;

		Keyboard = keyboard;
		Mouse = MouseState;

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

	public static int RgbToHex( int rgbColor )
	{
		int red = (rgbColor >> 16) & 0xFF;
		int green = (rgbColor >> 8) & 0xFF;
		int blue = rgbColor & 0xFF;

		return (red << 16) | (green << 8) | blue;
	}

	public static float ByteToFloat( byte byteValue )
	{
		return (float)byteValue / 255.0f;
	}

	public static byte FloatToByte( float floatValue )
	{
		return (byte)Math.Round( floatValue * 255.0f );
	}
}
