using System.Diagnostics;
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

public struct Vertex
{
	public Assimp.Vector3D Position;
	public Assimp.Vector3D Normal;
	public Assimp.Vector3D UV;

	public static int Size => sizeof( float ) * 9;
}

public sealed class VertexBuffer
{
	public int VAO { get; private set; }
	public int VBO { get; private set; }
	public int EBO { get; private set; }

	public VertexBuffer()
	{
		VAO = GL.GenVertexArray();
		GL.BindVertexArray( VAO );

		VBO = GL.GenBuffer();
		EBO = GL.GenBuffer();
	}

	public void Draw( Assimp.Vector3D[] vertices, uint[]? indices, Material shader, ref Matrix4 model, bool wireFrame = false, params ShaderUniformVariable[] uniforms )
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

	public void DrawModel( ModelComponent model, ref Matrix4 matrix, bool wireFrame = false, params ShaderUniformVariable[] uniforms )
	{
		// TODO: I dunno I need to fix the vertices on ModelComponent.
		var hackyVerts = new List<Assimp.Vector3D>();
		foreach ( var x in model.Positions )
		{
			hackyVerts.Add( x );
		}

		foreach ( var y in model.Normals )
		{
			hackyVerts.Add( y );
		}

		var hackyVertsArray = hackyVerts.ToArray();

		GL.BindVertexArray( VAO );
		{
			GL.BindBuffer( BufferTarget.ArrayBuffer, VBO );
			GL.BufferData(
				BufferTarget.ArrayBuffer,
				sizeof( float ) * 3 * hackyVertsArray.Length,
				hackyVertsArray,
				BufferUsageHint.DynamicDraw
			);

			if ( model.Indices.Length > 0 )
			{
				GL.BindBuffer( BufferTarget.ElementArrayBuffer, EBO );
				GL.BufferData(
					BufferTarget.ElementArrayBuffer,
					sizeof( uint ) * model.IndexCount,
					model.Indices,
					BufferUsageHint.DynamicDraw
				);
			}

			if ( model.Material is null )
				throw new Exception( "Material is null you messed up!" );

			BindMaterial( model.Material, ref matrix, uniforms );

			if ( model.Indices.Length > 0 )
				GL.DrawElements(
					wireFrame ? PrimitiveType.Lines : PrimitiveType.Triangles,
					model.IndexCount,
					DrawElementsType.UnsignedInt,
					0
				);
			else
				GL.DrawArrays(
					wireFrame ? PrimitiveType.Lines : PrimitiveType.Triangles,
					0,
					model.VertexCount
				);
		}
		GL.BindBuffer( BufferTarget.ArrayBuffer, 0 );
		GL.BindBuffer( BufferTarget.ElementArrayBuffer, 0 );
		GL.BindVertexArray( 0 );
		GL.UseProgram( 0 );
	}

	public void DrawScreenQuad( Screen screen )
	{
		var vertices = screen.Vertices;
		var material = screen.Material;

		GL.BindVertexArray( VAO );

		GL.BindTexture( TextureTarget.Texture2D, screen.ColorTexture.Handle );
		{
			GL.BindBuffer( BufferTarget.ArrayBuffer, VBO );
			GL.BufferData(
				BufferTarget.ArrayBuffer,
				sizeof( float ) * vertices.Length,
				vertices,
				BufferUsageHint.DynamicDraw
			);

			var handle = material.ProgramHandle;
			GL.UseProgram( handle );

			if ( !material.TryGetAttribLocation( "aPos", out var aPos ) )
				throw new Exception( $"Expect a vertex position attribute in material: {material}" );

			GL.EnableVertexAttribArray( aPos );
			GL.VertexAttribPointer( aPos, 2, VertexAttribPointerType.Float, false, 4 * sizeof( float ), 0 );

			if ( !material.TryGetAttribLocation( "aTexCoords", out var aTexCoords ) )
				throw new Exception( $"Expect a tex coord attribute in material: {material}" );

			GL.EnableVertexAttribArray( aTexCoords );
			GL.VertexAttribPointer( aTexCoords, 2, VertexAttribPointerType.Float, false, 4 * sizeof( float ), 2 * sizeof( float ) );

			GL.Disable( EnableCap.CullFace );
			GL.Disable( EnableCap.DepthTest );

			GL.DrawArrays(
				PrimitiveType.Triangles,
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

		if ( !material.TryGetAttribLocation( "aPos", out var aPos ) )
			throw new Exception( $"Expect a vertex position attribute in material: {material}" );

		if ( material.TryGetAttribLocation( "aNormal", out var aNormal ) )
		{
			GL.EnableVertexAttribArray( aPos );
			GL.VertexAttribPointer( aPos, 3, VertexAttribPointerType.Float, false, 3 * sizeof( float ), 0 );

			GL.EnableVertexAttribArray( aNormal );
			GL.VertexAttribPointer(
				aNormal,
				3,
				VertexAttribPointerType.Float,
				false,
				3 * sizeof( float ),
				0
			);

			if ( material.TryGetAttribLocation( "aTexCoords", out var aTexCoords ) )
			{
				GL.EnableVertexAttribArray( aTexCoords );
				GL.VertexAttribPointer( aTexCoords, 3, VertexAttribPointerType.Float, false, 3 * sizeof( float ), 0 );
			}
		}
		else
		{
			GL.EnableVertexAttribArray( aPos );
			GL.VertexAttribPointer( aPos, 3, VertexAttribPointerType.Float, false, 3 * sizeof( float ), 0 );
		}


		if ( uniforms.Length <= 0 )
			return;

		foreach ( var u in uniforms )
		{
			switch ( u.Value )
			{
				case float floatValue:
					material.TrySetUniformFloat( u.Name, floatValue );
					break;
				case Vector2 vec2Value:
					material.TrySetUniformVector2( u.Name, vec2Value );
					break;
				case Vector3 vec3Value:
					material.TrySetUniformVector3( u.Name, vec3Value );
					break;
				case Color4 color4Value:
					material.TrySetUniformColor4( u.Name, color4Value );
					break;
				case Matrix4 mat4Value:
					material.TrySetUniformMatrix4( u.Name, ref mat4Value );
					break;
				case Texture texValue:
					material.TrySetUniformSampler2D( u.Name, texValue );
					break;
			}
		}
	}
}
