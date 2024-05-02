using OpenTK.Windowing.Common;

namespace Sour;

public static class DebugDraw
{
	private struct DebugDrawMission
	{
		public Assimp.Vector3D[] Vertices;
		public Color4 Color;
		public float? Duration;
		public Matrix4? Matrix;
	}

	static Queue<DebugDrawMission> missions;

	static VertexBuffer vb;
	static Material DebugDrawShader;

	public static void Init()
	{
		missions = new();
		vb = new();
		DebugDrawShader = new( "Resources/Shaders/DebugDraw/debugdraw.vert", "Resources/Shaders/DebugDraw/debugdraw.frag" );
	}

	public static void Render( FrameEventArgs args )
	{
		while ( missions.TryDequeue( out var mission ) )
		{
			var matrix = mission.Matrix is null ? Matrix4.Identity : mission.Matrix.Value;
			vb.Draw( mission.Vertices, null, DebugDrawShader, ref matrix,
				wireFrame: true,
				new( "time", Time.Elapsed ),
				new( "color", mission.Color ),
				new( "dashSize", 10f ),
				new( "gapSize", 10f ),
				new( "resolution", Game.ScreenSize ) );

			if ( mission.Duration.HasValue )
			{
				mission.Duration -= Time.Delta;
				if ( mission.Duration > 0.0f )
				{
					missions.Enqueue( mission );
					return;
				}
			}
		}
	}

	public static void Line( Vector3 start, Vector3 end, Color4 color, float duration = 0, Matrix4? model = null )
	{
		var verts = new Assimp.Vector3D[2] { new Assimp.Vector3D( start.X, start.Y, start.Z ), new Assimp.Vector3D( end.X, end.Y, end.Z ) };
		missions.Enqueue( new DebugDrawMission
		{
			Vertices = verts,
			Color = color,
			Duration = duration > 0 ? duration : null,
			Matrix = model
		} );
	}

	public static void Box3( Box3 box, Color4 color, float duration = 0, Matrix4? model = null )
	{
		void drawLine( Vector3 start, Vector3 end )
		{
			var verts = new Assimp.Vector3D[2] { new Assimp.Vector3D( start.X, start.Y, start.Z ), new Assimp.Vector3D( end.X, end.Y, end.Z ) };
			missions.Enqueue( new DebugDrawMission
			{
				Vertices = verts,
				Color = color,
				Duration = duration > 0 ? duration : null,
				Matrix = model
			} );
		}


		Vector3 a = new Vector3( box.Min.X, box.Min.Y, box.Min.Z );
		Vector3 b = new Vector3( box.Max.X, box.Min.Y, box.Min.Z );
		Vector3 b2 = new Vector3( box.Max.X, box.Max.Y, box.Min.Z );
		Vector3 b3 = new Vector3( box.Min.X, box.Max.Y, box.Min.Z );
		Vector3 b4 = new Vector3( box.Min.X, box.Min.Y, box.Max.Z );
		Vector3 b5 = new Vector3( box.Max.X, box.Min.Y, box.Max.Z );
		Vector3 b6 = new Vector3( box.Max.X, box.Max.Y, box.Max.Z );
		Vector3 b7 = new Vector3( box.Min.X, box.Max.Y, box.Max.Z );
		drawLine( a, b );
		drawLine( b, b2 );
		drawLine( b2, b3 );
		drawLine( b3, a );
		drawLine( a, b4 );
		drawLine( b, b5 );
		drawLine( b2, b6 );
		drawLine( b3, b7 );
		drawLine( b4, b5 );
		drawLine( b5, b6 );
		drawLine( b6, b7 );
		drawLine( b7, b4 );
	}
}
