using OpenTK.Graphics.OpenGL4;

namespace Sour;

public class ShaderProgram
{
	public const string DefaultVertexShaderPath = "Resources/Shaders/vert.glsl";
	public const string DefaultFragmentShaderPath = "Resources/Shaders/frag.glsl";
	public int ProgramHandle;

	public Shader Vertex;
	public Shader Fragment;

	FileSystemWatcher watcher;

	public static Action OnFileChanged;

	public ShaderProgram( string vtxShaderPath, string fragShaderPath )
	{
		Vertex = new( ShaderType.VertexShader, vtxShaderPath );
		Fragment = new( ShaderType.FragmentShader, fragShaderPath );

		Reload();

		var log = GL.GetProgramInfoLog( ProgramHandle );
		Console.WriteLine( log );

		watcher = new FileSystemWatcher( Path.GetDirectoryName( vtxShaderPath ) );
		watcher.EnableRaisingEvents = true;
		watcher.Changed += Watcher_Changed;
	}

	private void Watcher_Changed( object sender, FileSystemEventArgs e )
	{
		OnFileChanged?.Invoke();
	}

	public void Reload()
	{
		if ( ProgramHandle != 0 )
			GL.DeleteProgram( ProgramHandle );

		ProgramHandle = GL.CreateProgram();

		int vertexHandle = Vertex.Load();
		int fragHandle = Fragment.Load();

		GL.AttachShader( ProgramHandle, vertexHandle );
		GL.AttachShader( ProgramHandle, fragHandle );

		GL.LinkProgram( ProgramHandle );

		GL.DetachShader( ProgramHandle, vertexHandle );
		GL.DetachShader( ProgramHandle, fragHandle );

		GL.DeleteShader( vertexHandle );
		GL.DeleteShader( fragHandle );
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
