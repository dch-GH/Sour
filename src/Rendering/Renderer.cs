using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Sour;
public struct RenderJob
{
	public Assimp.Vector3D[] Vertices;
	public uint[] Indices;
	public int ShaderHandle;
	public Matrix4 ModelMatrix;
}

internal class Renderer
{
	Matrix4 viewMatrix;
	Matrix4 projectionMatrix;

	int VAO;
	int VBO;
	int EBO;

	Shader defaultShader;
	bool wireFrame = false;
	Camera cam;
	Queue<RenderJob> renderJobs;

	public Renderer( SourWindow window, Camera cam )
	{
		this.cam = cam;
		renderJobs = new();

		VAO = GL.GenVertexArray();
		GL.BindVertexArray( VAO );

		VBO = GL.GenBuffer();
		EBO = GL.GenBuffer();

		defaultShader = new Shader( "Resources/Shaders/vert.glsl", "Resources/Shaders/frag.glsl" );
		defaultShader.Use();

		// Set up matrices
		viewMatrix = Matrix4.LookAt( cam.Transform.Position, Vector3.Zero, Vector3.UnitY );
		projectionMatrix = Matrix4.CreatePerspectiveFieldOfView( MathHelper.PiOver4, window.ClientSize.X / (float)window.ClientSize.Y, 0.1f, 100.0f );
	}

	public void Render( FrameEventArgs args )
	{
		GL.ClearColor( 0.05f, 0.25f, 0.3f, 1 );
		GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
		if ( renderJobs.Count <= 0 )
			return;


		while ( renderJobs.TryDequeue( out var currentJob ) )
		{
			if ( float.IsNaN( currentJob.ModelMatrix.Column0[0] ) )
				Log.Info( "ow" );

			Matrix4 modelViewProjection = currentJob.ModelMatrix * viewMatrix * projectionMatrix;
			int mvpLocation = GL.GetUniformLocation( defaultShader.ProgramHandle, "modelViewProjection" );
			GL.UniformMatrix4( mvpLocation, false, ref modelViewProjection );

			GL.BindVertexArray( VAO );
			{
				GL.BindBuffer( BufferTarget.ArrayBuffer, VBO );
				GL.BufferData( BufferTarget.ArrayBuffer, sizeof( float ) * 3 * currentJob.Vertices.Length, currentJob.Vertices, BufferUsageHint.DynamicDraw );

				if ( currentJob.Indices.Length > 0 )
				{
					GL.BindBuffer( BufferTarget.ElementArrayBuffer, EBO );
					GL.BufferData( BufferTarget.ElementArrayBuffer, sizeof( uint ) * currentJob.Indices.Length, currentJob.Indices, BufferUsageHint.DynamicDraw );
				}

				GL.VertexAttribPointer( 0, 3, VertexAttribPointerType.Float, false, 3 * sizeof( float ), 0 );
				GL.EnableVertexAttribArray( 0 );

				//GL.Disable( EnableCap.CullFace );
				if ( currentJob.Indices.Length > 0 )
					GL.DrawElements( wireFrame ? PrimitiveType.Lines : PrimitiveType.Triangles, currentJob.Indices.Length, DrawElementsType.UnsignedInt, 0 );
				else
					GL.DrawArrays( wireFrame ? PrimitiveType.Lines : PrimitiveType.Triangles, 0, currentJob.Vertices.Length );

				GL.BindBuffer( BufferTarget.ArrayBuffer, 0 );
				GL.BindBuffer( BufferTarget.ElementArrayBuffer, 0 );
				GL.BindVertexArray( 0 );
			}
		}
	}

	public void Update( FrameEventArgs args, KeyboardState keyboard )
	{
		var dt = ((float)args.Time);

		if ( keyboard.IsKeyReleased( Keys.Z ) )
			wireFrame = !wireFrame;

		// Update view matrix based on camera position and orientation
		viewMatrix = Matrix4.LookAt( cam.Transform.Position, cam.Transform.Position + cam.Transform.Forward, Vector3.UnitY );
	}

	public void PushJob( RenderJob job )
	{
		renderJobs.Enqueue( job );
	}
}
