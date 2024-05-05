using System.Diagnostics;
using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Sour;

public struct ModelDrawMission
{
	public ModelComponent Model;
	public Matrix4 Matrix;
}

public class ModelRenderer
{
	public static Material DefaultShader;

	bool wireFrame = false;
	Camera camera;

	Queue<ModelDrawMission> modelMissions;
	VertexBuffer vb;

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
	}

	public void PushModelMission( ModelDrawMission mission )
	{
		modelMissions.Enqueue( mission );
	}

	private void DrawModel( ModelDrawMission mission )
	{
		Debug.Assert( mission.Model.Material is not null );
		// vb.Draw( model.Vertices, model.Indices, model.Material is null ? DefaultShader : model.Material, ref mission.Matrix, wireFrame, [new( "time", Time.Elapsed )] );
		vb.DrawModel( mission.Model, ref mission.Matrix, wireFrame, [new ShaderUniformVariable( "aObjectId", mission.Model.GameObject.Id )] );
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


