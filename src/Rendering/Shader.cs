using OpenTK.Graphics.OpenGL4;

namespace Sour;

public struct Shader
{
	public ShaderType ShaderType;
	public int Handle;

	public readonly string Path;
	private string lastWorkingSource;

	public Shader( ShaderType shaderType, string path )
	{
		ShaderType = shaderType;
		Path = path;
	}

	public int Load()
	{
		var handle = GL.CreateShader( ShaderType );

		var shaderFile = File.Open( Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite );
		var sr = new StreamReader( shaderFile );
		var shaderSource = sr.ReadToEnd();

		GL.ShaderSource( handle, shaderSource );
		GL.CompileShader( handle );

		var log = GL.GetShaderInfoLog( handle );
		if ( log.Length > 0 )
		{
			Console.WriteLine( $"Error compiling shader: {Path}" );
			Console.WriteLine( log );

			GL.ShaderSource( handle, lastWorkingSource );
			GL.CompileShader( handle );
		}
		else
		{
			lastWorkingSource = shaderSource;
			Console.WriteLine( $"Yay we compiled {ShaderType}: {Path} :)" );
		}

		sr.Close();
		shaderFile.Close();
		return handle;
	}
}
