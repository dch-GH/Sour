﻿using System.Diagnostics.Contracts;
using System.Reflection.Metadata;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Sour;

public struct RenderJob
{
	public Model Model;
	public Matrix4 Matrix;
}

public class Renderer
{
	public static Shader DefaultShader;
	GameWindow window;
	Matrix4 viewMatrix;
	Matrix4 projectionMatrix;

	int VAO;
	int VBO;
	int EBO;

	bool wireFrame = false;
	Camera cam;
	Queue<RenderJob> renderJobs;

	Vector3 lightPosition = new Vector3( -2, -16, 2 );
	public bool ShadersNeedReloading = false;

	public Renderer( SourWindow window, Camera cam )
	{
		this.window = window;
		this.cam = cam;
		renderJobs = new();

		VAO = GL.GenVertexArray();
		GL.BindVertexArray( VAO );

		VBO = GL.GenBuffer();
		EBO = GL.GenBuffer();

		DefaultShader = new Shader(
			Shader.DefaultVertexShaderPath,
			Shader.DefaultFragmentShaderPath
		);

		viewMatrix = Matrix4.LookAt( cam.Transform.Position, Vector3.Zero, Vector3.UnitY );
		projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(
			MathHelper.DegreesToRadians( 90 ),
			window.ClientSize.X / (float)window.ClientSize.Y,
			0.1f,
			300.0f
		);

		Shader.OnFileChanged += () => { ShadersNeedReloading = true; };
	}

	public void Render( FrameEventArgs args )
	{
		GL.ClearColor( 0.05f, 0.25f, 0.3f, 1 );
		GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
		if ( renderJobs.Count <= 0 )
			return;

		if ( ShadersNeedReloading )
		{
			Log.Info( "Reloading shaders..." );
			while ( renderJobs.TryDequeue( out var currentJob ) )
			{
				currentJob.Model.ReloadShaders();
			}
			ShadersNeedReloading = false;
			return;
		}

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

		// Update view matrix based on camera position and orientation
		viewMatrix = Matrix4.LookAt(
			cam.Transform.Position,
			cam.Transform.Position + cam.Transform.Forward,
			Vector3.UnitY
		);
		projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(
			MathHelper.DegreesToRadians( cam.FieldOfView ),
			window.ClientSize.X / (float)(window.ClientSize.Y),
			0.1f,
			100.0f
		);
	}

	public void PushJob( RenderJob job )
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

			var shaderHandle = job.Model.Shader is null ? DefaultShader.ProgramHandle : job.Model.Shader.ProgramHandle;
			UseShader( job.Model.Shader, ref job.Matrix );

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

	private void UseShader( Shader shader, ref Matrix4 model )
	{
		var handle = shader.ProgramHandle;
		GL.UseProgram( handle );

		shader.SetUniformMatrix4( "model", ref model );
		shader.SetUniformMatrix4( "view", ref viewMatrix );
		shader.SetUniformMatrix4( "projection", ref projectionMatrix );

		var aPos = GL.GetAttribLocation( handle, "aPos" );
		GL.EnableVertexAttribArray( aPos );
		GL.VertexAttribPointer( aPos, 3, VertexAttribPointerType.Float, false, 6 * sizeof( float ), 0 );

		var aNormal = GL.GetAttribLocation( handle, "aNormal" );
		GL.EnableVertexAttribArray( aNormal );
		GL.VertexAttribPointer(
			aNormal,
			3,
			VertexAttribPointerType.Float,
			false,
			6 * sizeof( float ),
			0
		);

		lightPosition = cam.Transform.Position + Vector3.UnitY * 2;
		var lightPosUniform = GL.GetUniformLocation( handle, "lightPos" );
		if ( lightPosUniform != -1 )
			GL.Uniform3( lightPosUniform, lightPosition );
	}

	public static void CheckGLError()
	{
		var err = GL.GetError();
		if ( err != OpenTK.Graphics.OpenGL4.ErrorCode.NoError )
		{
			//Log.Info( err );
			//throw new Exception( err.ToString() );
		}
	}
}

