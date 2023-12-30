using OpenTK.Graphics.OpenGL4;
using System.Text;

namespace Sour;

internal class Shader
{
	public int ProgramHandle;

	public int VertexHandle;
	public int FragmentHandle;

	public Shader( string vtxShaderPath, string fragShaderPath )
	{
		ProgramHandle = GL.CreateProgram();

		VertexHandle = LoadShader( ShaderType.VertexShader, vtxShaderPath );
		FragmentHandle = LoadShader( ShaderType.FragmentShader, fragShaderPath );

		GL.AttachShader( ProgramHandle, VertexHandle );
		GL.AttachShader( ProgramHandle, FragmentHandle );
		GL.LinkProgram( ProgramHandle );
		GL.DeleteShader( VertexHandle );
		GL.DeleteShader( FragmentHandle );

		var log = GL.GetProgramInfoLog( ProgramHandle );
		Console.WriteLine( log );
	}

	private int LoadShader( ShaderType shaderType, string path )
	{
		var handle = GL.CreateShader( shaderType );
		var shaderSource = File.ReadAllLines( path );
		var sb = new StringBuilder();
		foreach ( var x in shaderSource )
			sb.Append( $"{x}\n" );

		GL.ShaderSource( handle, sb.ToString() );
		GL.CompileShader( handle );

		var log = GL.GetShaderInfoLog( handle );
		if ( log.Length > 0 )
		{
			Console.WriteLine( $"Error compiling shader: {shaderType}" );
			Console.WriteLine( log );
		}
		else
			Console.WriteLine( $"Yay we compiled the {shaderType} :)" );

		return handle;
	}

	public void Use()
	{
		GL.UseProgram( ProgramHandle );
	}
}
