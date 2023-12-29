using OpenTK.Graphics.OpenGL4;
using System.Text;

namespace Sour;

internal class Shader
{
	public int Handle;
	public ShaderType ShadeType;

	public Shader( ShaderType shaderType, string path )
	{
		Handle = GL.CreateShader( shaderType );
		var shaderSource = File.ReadAllLines( path );
		var sb = new StringBuilder();
		foreach ( var x in shaderSource )
			sb.Append( $"{x}\n" );

		GL.ShaderSource( Handle, sb.ToString() );
		GL.CompileShader( Handle );

		var log = GL.GetShaderInfoLog( Handle );
		if ( log.Length > 0 )
		{
			Console.WriteLine( $"Error compiling shader: {shaderType}" );
			Console.WriteLine( log );
		}
		else
			Console.WriteLine( $"Yay we compiled the {shaderType} :)" );
	}
}
