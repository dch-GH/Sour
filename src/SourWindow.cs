using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System.Text;

namespace Sour;

internal sealed class SourWindow : GameWindow
{
	float[] vertices = {
	-0.5f, -0.5f, 0.0f, //Bottom-left vertex
     0.5f, -0.5f, 0.0f, //Bottom-right vertex
     0.0f,  0.5f, 0.0f  //Top vertex
	};

	int VAO;
	int VBO;
	int shaderProgram;

	Shader vertexShader;
	Shader fragmentShader;

	public SourWindow( GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings ) : base( gameWindowSettings, nativeWindowSettings )
	{
	}
	
	protected override void OnLoad()
	{
		base.OnLoad();

		VAO = GL.GenVertexArray();

		GL.BindVertexArray( VAO );

		VBO = GL.GenBuffer();
		GL.BindBuffer( BufferTarget.ArrayBuffer, VBO );
		GL.BufferData( BufferTarget.ArrayBuffer, sizeof( float ) * vertices.Length, vertices, BufferUsageHint.StaticDraw );

		vertexShader = new( OpenTK.Graphics.OpenGL4.ShaderType.VertexShader, "Resources/Shaders/vert.glsl" );
		fragmentShader = new( OpenTK.Graphics.OpenGL4.ShaderType.FragmentShader, "Resources/Shaders/frag.glsl" );

		shaderProgram = GL.CreateProgram();
		GL.AttachShader( shaderProgram, vertexShader.Handle );
		GL.AttachShader( shaderProgram, fragmentShader.Handle );
		GL.LinkProgram( shaderProgram );
		GL.DeleteShader( vertexShader.Handle );
		GL.DeleteShader( fragmentShader.Handle );

		var log = GL.GetProgramInfoLog( shaderProgram );
		Console.WriteLine( log );

		GL.VertexAttribPointer( 0, 3, VertexAttribPointerType.Float, false, 3 * sizeof( float ), 0 );
		GL.EnableVertexAttribArray( 0 );
		GL.UseProgram( shaderProgram );
	}

	public override void Run()
	{
		base.Run();
	}

	protected override void OnRenderFrame( FrameEventArgs args )
	{
		base.OnRenderFrame( args );
		GL.ClearColor( 0.05f, 0.25f, 0.3f, 1 );
		GL.Clear( ClearBufferMask.ColorBufferBit );

		GL.BindVertexArray( VAO );
		GL.DrawArrays( PrimitiveType.Triangles, 0, 3 );
		SwapBuffers();
	}

	protected override void OnResize( ResizeEventArgs e )
	{
		base.OnResize( e );
		GL.Viewport( 0, 0, e.Width, e.Height );
	}
}
