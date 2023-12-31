using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;
using System.Text;

namespace Sour;

public class Shader
{
	public const string DefaultVertexShaderPath = "Resources/Shaders/vert.glsl";
	public const string DefaultFragmentShaderPath = "Resources/Shaders/frag.glsl";
	public int ProgramHandle;

	public int VertexHandle;
	public int FragmentHandle;

	readonly string vtxPath;
	readonly string fragpath;

	public Shader( string vtxShaderPath, string fragShaderPath )
	{
		vtxPath = vtxShaderPath;
		fragpath = fragShaderPath;

		Reload();

		var log = GL.GetProgramInfoLog( ProgramHandle );
		Console.WriteLine( log );
	}

	public void Reload()
	{
		ProgramHandle = GL.CreateProgram();

		VertexHandle = LoadShader( ShaderType.VertexShader, vtxPath );
		FragmentHandle = LoadShader( ShaderType.FragmentShader, fragpath );

		GL.AttachShader( ProgramHandle, VertexHandle );
		GL.AttachShader( ProgramHandle, FragmentHandle );

		GL.LinkProgram( ProgramHandle );

		//GL.DetachShader( ProgramHandle, VertexHandle );
		//GL.DetachShader( ProgramHandle, FragmentHandle );

		// GL.DeleteShader( VertexHandle );
		// GL.DeleteShader( FragmentHandle );
	}

	private int LoadShader( ShaderType shaderType, string path )
	{
		var handle = GL.CreateShader( shaderType );

		var shaderSource = File.ReadAllText( path );
		GL.ShaderSource( handle, shaderSource );
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

	public bool SetUniformMatrix4( string name, ref Matrix4 matrix )
	{
		var location = GL.GetUniformLocation( ProgramHandle, name );
		if ( location < 0 )
		{
			throw new Exception();
		}
		GL.UniformMatrix4( location, false, ref matrix );
		Renderer.CheckGLError();
		return true;
	}
}
