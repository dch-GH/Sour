using System.Drawing;
using OpenTK.Platform.Windows;
using OpenTK.Windowing.Common;

namespace Sour;

public static class DebugDraw
{
	private struct DebugLineJob
	{
		public Vector3 Start;
		public Vector3 End;
		public Color Color;
		public float Duration;
	}

	static Queue<DebugLineJob> lines;

	static VertexBuffer vb;
	static ShaderProgram DebugDrawShader;

	public static void Init()
	{
		lines = new();
		vb = new();
		DebugDrawShader = new( "Resources/Shaders/DebugDraw/debugdraw.vert", "Resources/Shaders/DebugDraw/debugdraw.frag" );
	}

	public static void Update( FrameEventArgs args )
	{
		while ( lines.TryDequeue( out var line ) )
		{
			var start = line.Start;
			var end = line.End;

			var matrix = Matrix4.CreateTranslation( Vector3.Zero ) * Matrix4.CreateFromQuaternion( Quaternion.Identity );
			var verts = new Assimp.Vector3D[2] { new Assimp.Vector3D( start.X, start.Y, start.Z ), new Assimp.Vector3D( end.X, end.Y, end.Z ) };
			vb.Draw( verts, null, DebugDrawShader, ref matrix );
		}
	}

	public static void Line( Vector3 start, Vector3 end, Color color, float duration = 0 )
	{
		lines.Enqueue( new DebugLineJob
		{
			Start = start,
			End = end,
		} );
	}
}
