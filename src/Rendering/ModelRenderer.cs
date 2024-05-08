using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Sour;

public struct ModelDrawMission
{
	public ModelComponent Model;
	public Matrix4 Matrix;
}

internal sealed class ModelRenderer : IRender
{
	public static Material DefaultShader;

	private bool wireFrame = false;
	private Queue<ModelDrawMission> modelMissions;
	private VertexBuffer vb;

	public ModelRenderer()
	{
		modelMissions = new();
		vb = new();
		DefaultShader = new Material(
			Material.DefaultVertexShaderPath,
			Material.DefaultFragmentShaderPath
		);

		Engine.UpdateEmitter.OnUpdateStage += Update;
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

	private void Update( UpdateStage stage, FrameEventArgs args )
	{
		if ( Engine.Keyboard.IsKeyReleased( Keys.Z ) )
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
		vb.DrawModel( mission.Model, ref mission.Matrix, wireFrame, [new ShaderUniformVariable( "aObjectId", mission.Model.GameObject.ColorId )] );
		CheckGLError();
	}

	public static void CheckGLError()
	{
		var err = GL.GetError();
		if ( err != OpenTK.Graphics.OpenGL4.ErrorCode.NoError )
		{
			// TODO: Getting InvalidValue here, but it doesn't break anything.
			Log.InfoInternal( err );
			//throw new Exception( err.ToString() );
		}
	}
}


