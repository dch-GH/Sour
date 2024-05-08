using System.Net;
using OpenTK.Graphics.OpenGL4;

namespace Sour;

public class Material
{
	public const string DefaultVertexShaderPath = "Resources/Shaders/vert.glsl";
	public const string DefaultFragmentShaderPath = "Resources/Shaders/frag.glsl";
	public static Material Defualt = new( DefaultVertexShaderPath, DefaultFragmentShaderPath );
	public int ProgramHandle;
	public Shader Vertex;
	public Shader Fragment;
	public Texture? Texture;

	FileSystemWatcher watcher;

	public Action OnFileChanged;
	public bool NeedsHotload;

	public Material( string? vtxShaderPath = null, string? fragShaderPath = null )
	{
		vtxShaderPath ??= DefaultVertexShaderPath;
		fragShaderPath ??= DefaultFragmentShaderPath;

		Vertex = new( ShaderType.VertexShader, vtxShaderPath );
		Fragment = new( ShaderType.FragmentShader, fragShaderPath );

		Reload();

		var log = GL.GetProgramInfoLog( ProgramHandle );
		Console.WriteLine( log );

		watcher = new FileSystemWatcher( Path.GetDirectoryName( vtxShaderPath ) );
		watcher.EnableRaisingEvents = true;
		watcher.Changed += Watcher_Changed;
		Engine.Materials.AddMaterial( this );
	}

	public Material WithTexture( Texture texture )
	{
		Texture = texture;
		return this;
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

	public bool TryGetUniformLocation( string name, out int uniformLocation )
	{
		uniformLocation = GL.GetUniformLocation( ProgramHandle, name );
		if ( uniformLocation < 0 )
			return false;

		return true;
	}

	public bool TryGetAttribLocation( string name, out int atribLocation )
	{
		atribLocation = GL.GetAttribLocation( ProgramHandle, name );
		if ( atribLocation < 0 )
			return false;

		return true;
	}

	public bool TrySetUniformFloat( string name, float value )
	{
		if ( TryGetUniformLocation( name, out var location ) )
		{
			GL.Uniform1( location, value );
			return true;
		}

		return false;
	}

	public bool TrySetUniformVector2( string name, OpenTK.Mathematics.Vector2 vec2 )
	{
		if ( TryGetUniformLocation( name, out var location ) )
		{
			GL.Uniform2( location, vec2 );
			return true;
		}

		return false;
	}

	public bool TrySetUniformVector3( string name, OpenTK.Mathematics.Vector3 vector3 )
	{
		if ( TryGetUniformLocation( name, out var location ) )
		{
			GL.Uniform3( location, vector3 );
			return true;
		}

		return false;
	}

	public bool TrySetUniformColor4( string name, Color4 color )
	{
		if ( TryGetUniformLocation( name, out var location ) )
		{
			GL.Uniform4( location, color );
			return true;
		}

		return false;
	}

	public bool TrySetUniformMatrix4( string name, ref Matrix4 matrix )
	{
		if ( TryGetUniformLocation( name, out var location ) )
		{
			GL.UniformMatrix4( location, false, ref matrix );
			return true;
		}

		return false;
	}

	public bool TrySetUniformSampler2D( string name, Texture texture )
	{
		if ( TryGetUniformLocation( name, out var location ) )
		{
			GL.Uniform1( location, texture.Handle );
			return true;
		}

		return false;
	}
}
