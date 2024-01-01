using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Sour;

public struct RenderJob
{
	public Model Model;
	public Matrix4 Matrix;
}

public class ModelRenderer
{
	public static Material DefaultShader;
	GameWindow window;

	int VAO;
	int VBO;
	int EBO;

	bool wireFrame = false;
	Camera cam;
	Queue<RenderJob> renderJobs;

	Vector3 lightPosition = new Vector3( -2, -16, 2 );

	public ModelRenderer( Game window, Camera cam )
	{
		this.window = window;
		this.cam = cam;
		renderJobs = new();

		VAO = GL.GenVertexArray();
		GL.BindVertexArray( VAO );

		VBO = GL.GenBuffer();
		EBO = GL.GenBuffer();

		DefaultShader = new Material(
			Material.DefaultVertexShaderPath,
			Material.DefaultFragmentShaderPath
		);
	}

	public void Render( FrameEventArgs args )
	{
		GL.ClearColor( 0.05f, 0.25f, 0.3f, 1 );
		GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
		if ( renderJobs.Count <= 0 )
			return;

		while ( renderJobs.TryDequeue( out var currentJob ) )
		{
			DoRenderJob( currentJob );
		}
	}

	public void Update( FrameEventArgs args, KeyboardState keyboard )
	{
		var dt = ((float)args.Time);

		if ( keyboard.IsKeyReleased( Keys.Z ) )
			wireFrame = !wireFrame;

		lightPosition = cam.Transform.Position + Vector3.UnitY * 2;
	}

	public void PushModel( RenderJob job )
	{
		renderJobs.Enqueue( job );
	}

	private void DoRenderJob( RenderJob job )
	{
		GL.Enable( EnableCap.CullFace );
		GL.BindVertexArray( VAO );
		{
			var vertices = job.Model.Vertices;
			var indices = job.Model.Indices;

			GL.BindBuffer( BufferTarget.ArrayBuffer, VBO );
			GL.BufferData(
				BufferTarget.ArrayBuffer,
				sizeof( float ) * 3 * vertices.Length,
				vertices,
				BufferUsageHint.DynamicDraw
			);

			if ( job.Model.Indices.Length > 0 )
			{
				GL.BindBuffer( BufferTarget.ElementArrayBuffer, EBO );
				GL.BufferData(
					BufferTarget.ElementArrayBuffer,
					sizeof( uint ) * indices.Length,
					indices,
					BufferUsageHint.DynamicDraw
				);
			}

			UseMaterial( job.Model.Material is null ? DefaultShader : job.Model.Material, ref job.Matrix );

			if ( indices.Length > 0 )
				GL.DrawElements(
					wireFrame ? PrimitiveType.Lines : PrimitiveType.Triangles,
					indices.Length,
					DrawElementsType.UnsignedInt,
					0
				);
			else
				GL.DrawArrays(
					wireFrame ? PrimitiveType.Lines : PrimitiveType.Triangles,
					0,
					vertices.Length
				);
		}
		GL.BindBuffer( BufferTarget.ArrayBuffer, 0 );
		GL.BindBuffer( BufferTarget.ElementArrayBuffer, 0 );
		GL.BindVertexArray( 0 );
		GL.UseProgram( 0 );
	}

	private void UseMaterial( Material material, ref Matrix4 model )
	{
		var handle = material.ProgramHandle;
		GL.UseProgram( handle );

		material.TrySetUniformMatrix4( "model", ref model );
		material.TrySetUniformMatrix4( "view", ref cam.ViewMatrix );
		material.TrySetUniformMatrix4( "projection", ref cam.ProjectionMatrix );

		var aPos = GL.GetAttribLocation( handle, "aPos" );
		GL.EnableVertexAttribArray( aPos );
		GL.VertexAttribPointer( aPos, 3, VertexAttribPointerType.Float, false, 6 * sizeof( float ), 0 );

		var aNormal = GL.GetAttribLocation( handle, "aNormal" );
		if ( aNormal != -1 )
		{
			GL.EnableVertexAttribArray( aNormal );
			GL.VertexAttribPointer(
				aNormal,
				3,
				VertexAttribPointerType.Float,
				false,
				6 * sizeof( float ),
				0
			);
		}

		material.TrySetUniform3( "lightPos", lightPosition );
		material.TrySetUniform1( "time", Time.Elapsed );
	}

	public static void CheckGLError()
	{
		var err = GL.GetError();
		if ( err != OpenTK.Graphics.OpenGL4.ErrorCode.NoError )
		{
			// TODO: Getting InvalidValue here, but it doesn't break anything.
			//Log.Info( err );
			//throw new Exception( err.ToString() );
		}
	}
}


