using System.Data.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Sour;

public sealed class Game : GameWindow
{
	public static Vector2i ScreenSize;
	public static KeyboardState Keyboard;
	public static MouseState Mouse;
	public static MaterialManager Materials;
	public static ModelRenderer ModelRenderer;
	public static UpdateEventEmitter UpdateEmitter;
	public static RenderEventEmitter RenderEmitter;

	private Camera _mainCamera;
	private Screen _screen;
	private Editor _editor;

	Vector3 lookAngles;
	float moveSpeed = 6;
	float lookSpeed = 3;

	public Game( GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings ) : base( gameWindowSettings, nativeWindowSettings )
	{
		UpdateEmitter = new();
		RenderEmitter = new();

		_mainCamera = new Camera( Vector3.Zero, Quaternion.Identity );
		Camera.Main = _mainCamera;

		_editor = new();
	}

	protected override void OnLoad()
	{
		base.OnLoad();
		ScreenSize = ClientSize;
		CenterWindow();

		Materials = new();

		_mainCamera.Transform.Position = new Vector3( 0, 0, -5 );
		DebugDraw.Init();
		ModelRenderer = new( this, _mainCamera );

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
			Materials.TryHotloadShaders();

		UpdateEmitter.OnUpdateStage?.Invoke( UpdateStage.PreRender, args );
		RenderEmitter.OnRenderStage?.Invoke( RenderStage.PreEverything, args );

		// GameObjects and Components submit drawtasks here, which are drawn later in this loop.
		GameObject.RenderAll();
		{
			_screen.PreDraw();

			if ( _editor.RequestObjectIdRender )
			{
				GL.DrawBuffers( 2, [DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1] );
			}
			else
				GL.DrawBuffer( DrawBufferMode.ColorAttachment0 );

			// Clear the scene.
			GL.ClearColor( 0.06f, 0.02f, 0.03f, 1.0f );
			GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

			GL.Enable( EnableCap.CullFace );
			GL.Enable( EnableCap.DepthTest );

			// Draw the scene.
			RenderEmitter.OnRenderStage?.Invoke( RenderStage.PreSceneRender, args );
			ModelRenderer.Render( args );
			RenderEmitter.OnRenderStage?.Invoke( RenderStage.PostSceneRender, args );

			GL.DrawBuffers( 1, [DrawBuffersEnum.ColorAttachment0] );
			GL.Clear( ClearBufferMask.None );

			RenderEmitter.OnRenderStage?.Invoke( RenderStage.DebugDraw, args );
			DebugDraw.Render( args );

			_screen.Draw();

			SwapBuffers();
		}

		UpdateEmitter.OnUpdateStage?.Invoke( UpdateStage.PostRender, args );
		Time.Delta = (float)args.Time;
		Time.Elapsed += Time.Delta;
	}

	protected override void OnResize( ResizeEventArgs e )
	{
		base.OnResize( e );
		ScreenSize = ClientSize;
		_screen.Resize( ScreenSize );
	}

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
			wishDir += _mainCamera.Transform.Forward;
		if ( keyboard.IsKeyDown( Keys.S ) )
			wishDir -= _mainCamera.Transform.Forward;

		if ( keyboard.IsKeyDown( Keys.A ) )
			wishDir += _mainCamera.Transform.Right;
		if ( keyboard.IsKeyDown( Keys.D ) )
			wishDir -= _mainCamera.Transform.Right;

		if ( wishDir.LengthSquared > 0 )
			wishDir.Normalize();

		wishDir *= moveSpeed * dt;

		if ( keyboard.IsKeyDown( Keys.Left ) )
			lookAngles += Vector3.UnitY * lookSpeed * dt;
		if ( keyboard.IsKeyDown( Keys.Right ) )
			lookAngles -= Vector3.UnitY * lookSpeed * dt;

		_mainCamera.Transform.Position += wishDir;
		_mainCamera.Transform.Rotation = Quaternion.FromEulerAngles( lookAngles );

	}
}
