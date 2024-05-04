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

			// Draw the scene.
			ModelRenderer.Render( args );
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
