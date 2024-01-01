using OpenTK.Graphics.OpenGL4;

namespace Sour;

public struct ShaderUniformVariable
{
	public string Name;
	public object Value;

	public ShaderUniformVariable( string name, object value )
	{
		Name = name;
		Value = value;
	}
}

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

	public void Draw( Assimp.Vector3D[] vertices, uint[]? indices, Material shader, ref Matrix4 model, params ShaderUniformVariable[] uniforms )
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

			BindMaterial( shader, ref model, uniforms );

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

	private void BindMaterial( Material material, ref Matrix4 model, params ShaderUniformVariable[] uniforms )
	{
		var handle = material.ProgramHandle;
		GL.UseProgram( handle );

		material.TrySetUniformMatrix4( "model", ref model );
		material.TrySetUniformMatrix4( "view", ref Camera.Main.ViewMatrix );
		material.TrySetUniformMatrix4( "projection", ref Camera.Main.ProjectionMatrix );

		var aPos = GL.GetAttribLocation( handle, "aPos" );
		GL.EnableVertexAttribArray( aPos );
		GL.VertexAttribPointer( aPos, 3, VertexAttribPointerType.Float, false, 3 * sizeof( float ), 0 );

		if ( uniforms.Length <= 0 )
			return;

		foreach ( var u in uniforms )
		{
			switch ( u.Value )
			{
				case float floatValue:
					material.TrySetUniform1( u.Name, floatValue );
					break;
				case Vector2 vec2Value:
					material.TrySetUniform2( u.Name, vec2Value );
					break;
				case Vector3 vec3Value:
					material.TrySetUniform3( u.Name, vec3Value );
					break;
				case Color4 color4Value:
					material.TrySetColor4( u.Name, color4Value );
					break;
				case Matrix4 mat4Value:
					material.TrySetUniformMatrix4( u.Name, ref mat4Value );
					break;
			}
		}
	}
}
