using OpenTK.Graphics.OpenGL4;

namespace Sour;

public sealed class VertexBuffer
{
	public int VAO { get; private set; }
	public int VBO { get; private set; }
	public int EBO { get; private set; }

	public bool Wireframe = true;

	public VertexBuffer()
	{
		VAO = GL.GenVertexArray();
		GL.BindVertexArray( VAO );

		VBO = GL.GenBuffer();
		EBO = GL.GenBuffer();

	}

	public void Draw( Assimp.Vector3D[] vertices, uint[]? indices, ShaderProgram shader, ref Matrix4 model )
	{
		GL.BindVertexArray( VAO );
		{
			GL.BindBuffer( BufferTarget.ArrayBuffer, VBO );
			GL.BufferData(
				BufferTarget.ArrayBuffer,
				sizeof( float ) * 3 * vertices.Length,
				vertices,
				BufferUsageHint.DynamicDraw
			);

			if ( indices is not null )
			{
				GL.BindBuffer( BufferTarget.ElementArrayBuffer, EBO );
				GL.BufferData(
					BufferTarget.ElementArrayBuffer,
					sizeof( uint ) * indices.Length,
					indices,
					BufferUsageHint.DynamicDraw
				);
			}

			// var shaderHandle = job.Model.Shader is null ? DefaultShader.ProgramHandle : job.Model.Shader.ProgramHandle;
			UseShader( shader, ref model );

			if ( indices is not null )
				GL.DrawElements(
					Wireframe ? PrimitiveType.Lines : PrimitiveType.Triangles,
					indices.Length,
					DrawElementsType.UnsignedInt,
					0
				);
			else
				GL.DrawArrays(
					Wireframe ? PrimitiveType.Lines : PrimitiveType.Triangles,
					0,
					vertices.Length
				);
		}
		GL.BindBuffer( BufferTarget.ArrayBuffer, 0 );
		GL.BindBuffer( BufferTarget.ElementArrayBuffer, 0 );
		GL.BindVertexArray( 0 );
		GL.UseProgram( 0 );
	}

	private void UseShader( ShaderProgram shader, ref Matrix4 model )
	{
		var handle = shader.ProgramHandle;
		GL.UseProgram( handle );

		shader.SetUniformMatrix4( "model", ref model );
		shader.SetUniformMatrix4( "view", ref Camera.Main.ViewMatrix );
		shader.SetUniformMatrix4( "projection", ref Camera.Main.ProjectionMatrix );

		var aPos = GL.GetAttribLocation( handle, "aPos" );
		GL.EnableVertexAttribArray( aPos );
		GL.VertexAttribPointer( aPos, 3, VertexAttribPointerType.Float, false, 3 * sizeof( float ), 0 );
	}
}
