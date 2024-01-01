using OpenTK.Graphics.OpenGL4;

namespace Sour;

public class Material
{
	public const string DefaultVertexShaderPath = "Resources/Shaders/vert.glsl";
	public const string DefaultFragmentShaderPath = "Resources/Shaders/frag.glsl";
	public int ProgramHandle;

	public Shader Vertex;
	public Shader Fragment;

	FileSystemWatcher watcher;

	public Action OnFileChanged;
	public bool NeedsHotload;

	public Material( string vtxShaderPath, string fragShaderPath )
	{
		Vertex = new( ShaderType.VertexShader, vtxShaderPath );
		Fragment = new( ShaderType.FragmentShader, fragShaderPath );

		Reload();

		var log = GL.GetProgramInfoLog( ProgramHandle );
		Console.WriteLine( log );

		watcher = new FileSystemWatcher( Path.GetDirectoryName( vtxShaderPath ) );
		watcher.EnableRaisingEvents = true;
		watcher.Changed += Watcher_Changed;
		Game.Materials.AddMaterial( this );
	}

	private void Watcher_Changed( object sender, FileSystemEventArgs e )
	{
		OnFileChanged?.Invoke();
		NeedsHotload = true;
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

	public bool TrySetUniformMatrix4( string name, ref Matrix4 matrix )
	{
		var location = GL.GetUniformLocation( ProgramHandle, name );
		if ( location < 0 )
			return false;

		GL.UniformMatrix4( location, false, ref matrix );
		return true;
	}

	public bool TrySetUniform1( string name, float value )
	{
		var location = GL.GetUniformLocation( ProgramHandle, name );
		if ( location < 0 )
			return false;

		GL.Uniform1( location, value );
		return true;
	}

	public bool TrySetUniform2( string name, OpenTK.Mathematics.Vector2 vec2 )
	{
		var location = GL.GetUniformLocation( ProgramHandle, name );
		if ( location < 0 )
			return false;

		GL.Uniform2( location, vec2 );
		return true;
	}

	public bool TrySetUniform3( string name, OpenTK.Mathematics.Vector3 vector3 )
	{
		var location = GL.GetUniformLocation( ProgramHandle, name );
		if ( location < 0 )
			return false;

		GL.Uniform3( location, vector3 );
		return true;
	}

	public bool TrySetColor4( string name, Color4 color )
	{
		var location = GL.GetUniformLocation( ProgramHandle, name );
		if ( location < 0 )
			return false;

		GL.Uniform4( location, color );
		return true;
	}
}
