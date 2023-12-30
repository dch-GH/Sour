using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Sour;

public struct Face
{
	public Vector3[] Vertices;
	public uint[] Indices;
}



internal sealed class SourWindow : GameWindow
{
	List<Assimp.Vector3D> importedVerts;
	List<uint> importedIndices;

	Vector3 lookAngles;
	Camera MainCamera = new();
	float moveSpeed = 6;
	float lookSpeed = 3;

	Face upFace;
	Renderer renderer;
	Model monkey;
	Model cone;

	public SourWindow( GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings ) : base( gameWindowSettings, nativeWindowSettings )
	{
	}

	protected override void OnLoad()
	{
		base.OnLoad();
		renderer = new( this, MainCamera );
		monkey = new Model( "Resources/Models/Suzanne/Suzanne.obj" );
		cone = new Model( "Resources/Models/Cone/cone.obj" );
		cone.Transform.Position += Vector3.UnitX * 1f;
		MainCamera.Transform.Position = new Vector3( 0, 0, -5 );
	}

	public override void Run()
	{
		base.Run();
	}

	protected override void OnRenderFrame( FrameEventArgs args )
	{
		base.OnRenderFrame( args );
		monkey.Render( renderer );
		cone.Render( renderer );
		renderer.Render( args );
		SwapBuffers();
	}

	protected override void OnResize( ResizeEventArgs e )
	{
		base.OnResize( e );
		GL.Viewport( 0, 0, e.Width, e.Height );
	}

	float accum = 0;
	protected override void OnUpdateFrame( FrameEventArgs args )
	{
		base.OnUpdateFrame( args );
		var dt = ((float)args.Time);
		var keyboard = KeyboardState;

		renderer.Update( args, keyboard );

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
		accum += dt;
		monkey.Transform.Rotation = Quaternion.FromAxisAngle( Vector3.UnitY, accum );
		cone.Transform.Rotation = Quaternion.FromAxisAngle( Vector3.UnitX, accum * 2 );
		MainCamera.Transform.Position += wishDir;
		MainCamera.Transform.Rotation = Quaternion.FromEulerAngles( lookAngles );

	}


	////	static Vector3[] vertices = {
	////	 new Vector3(0.5f,  0.5f, 0.0f),  // top right
	////    new Vector3( 0.5f, -0.5f, 0.0f),  // bottom right
	////   new Vector3( -0.5f, -0.5f, 0.0f),  // bottom left
	////   new Vector3( -0.5f,  0.5f, 0.0f)   // top left
	////};

	////	static uint[] indices = {  // note that we start from 0!
	////    0, 1, 3,   // first triangle
	////    1, 2, 3    // second triangle
	////};
	//public static Face MakeFaceUp( Vector3 origin, float size = 1 )
	//{
	//	var face = new Face();
	//	face.Vertices = new Vector3[4];
	//	face.Indices = new uint[6];

	//	//face.Vertices[0] = origin + new Vector3( size, size, 0 );
	//	//face.Vertices[1] = origin + new Vector3( size, -size, 0 );
	//	//face.Vertices[2] = origin + new Vector3( -size, -size, 0 );
	//	//face.Vertices[3] = origin + new Vector3( -size, size, 0 );

	//	//face.Indices[0] = 0;
	//	//face.Indices[1] = 1;
	//	//face.Indices[2] = 3;

	//	//face.Indices[3] = 1;
	//	//face.Indices[4] = 2;
	//	//face.Indices[5] = 3;

	//	//face.Vertices = vertices;
	//	//face.Indices = importedIndices;

	//	return face;
	//}
}
