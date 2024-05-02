using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Sour;

public struct ModelDrawMission
{
	public Model Model;
	public Matrix4 Matrix;
}

public class ModelRenderer
{
	public static Material DefaultShader;

	bool wireFrame = false;
	Camera camera;

	Queue<ModelDrawMission> modelMissions;
	VertexBuffer vb;

	Vector3 lightPosition = new Vector3( -2, -16, 2 );

	public ModelRenderer( Game window, Camera cam )
	{
		camera = cam;
		modelMissions = new();
		vb = new();
		DefaultShader = new Material(
			Material.DefaultVertexShaderPath,
			Material.DefaultFragmentShaderPath
		);
	}

	public void Render( FrameEventArgs args )
	{
		GL.ClearColor( 0.05f, 0.25f, 0.3f, 1 );
		GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
		if ( modelMissions.Count <= 0 )
			return;

		while ( modelMissions.TryDequeue( out var mission ) )
		{
			DrawModel( mission );
		}
	}

	public void Update( FrameEventArgs args, KeyboardState keyboard )
	{
		var dt = ((float)args.Time);

		if ( keyboard.IsKeyReleased( Keys.Z ) )
			wireFrame = !wireFrame;

		lightPosition = camera.Transform.Position;
	}

	public void PushModelMission( ModelDrawMission mission )
	{
		modelMissions.Enqueue( mission );
	}

	private void DrawModel( ModelDrawMission mission )
	{
		var model = mission.Model;

		GL.Enable( EnableCap.CullFace );
		GL.Enable( EnableCap.DepthTest );
		vb.Draw( model.Vertices, model.Indices, model.Material is null ? DefaultShader : model.Material,
			ref mission.Matrix,
			wireFrame,
			new( "lightPos", lightPosition ),
			new( "time", Time.Elapsed ) );
		CheckGLError();
	}

	public static void CheckGLError()
	{
		var err = GL.GetError();
		if ( err != OpenTK.Graphics.OpenGL4.ErrorCode.NoError )
		{
			// TODO: Getting InvalidValue here, but it doesn't break anything.
			Log.Info( err );
			//throw new Exception( err.ToString() );
		}
	}
}


