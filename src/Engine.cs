using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Sour;

public sealed class Engine : GameWindow
{
	public static Vector2i ScreenSize;
	public static FrameEventArgs CurrentFrameEvent;
	public static KeyboardState Keyboard;
	public static Mouse Mouse;
	public static UpdateEventEmitter UpdateEmitter;
	public static RenderEventEmitter RenderEmitter;

	internal static MaterialManager Materials;
	internal static ModelRenderer ModelRenderer;
	internal static bool DebugObjectIdMousePick;

	private FPSCounter _fpsCounter;
	private GameObject _mainCamera;
	private Screen _screen;
	private Editor _editor;

	internal Engine( GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings ) : base( gameWindowSettings, nativeWindowSettings )
	{
		UpdateEmitter = new();
		RenderEmitter = new();
		_editor = new();
		_fpsCounter = new();
	}

	protected override void OnLoad()
	{
		base.OnLoad();
		ScreenSize = ClientSize;
		CenterWindow();

		Materials = new();
		ModelRenderer = new();
		DebugDraw.Init();

		var screenMaterial = new Material( "Resources/Shaders/Screen/vert_screen.glsl", "Resources/Shaders/Screen/frag_screen.glsl" );
		_screen = new( ScreenSize, screenMaterial, new VertexBuffer() );

		// TODO: Make this easier to manage.
		CursorState = CursorState.Normal;
		Mouse.Visible = true;

		_mainCamera = GameObject.Spawn();
		_mainCamera.Transform.Position = new Vector3( 0, 0, -5 );
		_mainCamera.AddComponent( new Camera() );
		_mainCamera.AddComponent( new CameraController() );

		var cube = GameObject.Spawn();
		cube.AddComponent( new ModelComponent( "Resources/Models/Box/box.fbx" ) );
		cube.Transform.Rotation += Quaternion.FromAxisAngle( Axis.Right, 35f );

		var cone = GameObject.Spawn();
		cone.AddComponent( new ModelComponent( "Resources/Models/Cone/cone.obj", new Material( fragShaderPath: "Resources/Shaders/frag.glsl" ) ) );
		cone.Transform.Position += Axis.Right * 3f;
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
				// Only render to the ObjectId texture if we need it.
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
		Time.Frames += 1;
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
		CurrentFrameEvent = args;

		Keyboard = KeyboardState;
		Mouse.State = MouseState;
		Mouse.Delta = MouseState.Delta * Time.Delta;

		// The mouse gets snapped to the window so ignore the insane delta value.
		if ( Time.IsFirstFrame )
			Mouse.Delta = Vector2.Zero;

		if ( Keyboard.IsKeyReleased( Keys.Tab ) )
		{
			CursorState = CursorState == CursorState.Grabbed ? CursorState.Normal : CursorState.Grabbed;

			// TODO: Make this easier to manage.
			Mouse.Visible = CursorState == CursorState.Normal;
			Mouse.Delta = Vector2.Zero;
		}

		UpdateEmitter.OnUpdateStage?.Invoke( UpdateStage.PreUpdate, args );

		GameObject.UpdateAll();
		UpdateEmitter.OnUpdateStage?.Invoke( UpdateStage.Update, args );

		UpdateEmitter.OnUpdateStage?.Invoke( UpdateStage.PostUpdate, args );
	}
}
