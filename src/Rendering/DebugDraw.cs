using OpenTK.Windowing.Common;

namespace Sour;

public static class DebugDraw
{
	private struct DebugDrawMission
	{
		public Assimp.Vector3D[] Vertices;
		public Color4 Color;
		public float? Duration;
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

	public static void Update( FrameEventArgs args )
	{
		while ( missions.TryDequeue( out var mission ) )
		{
			var matrix = Matrix4.CreateTranslation( Vector3.Zero ) * Matrix4.CreateFromQuaternion( Quaternion.Identity );
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

	public static void Line( Vector3 start, Vector3 end, Color4 color, float duration = 0 )
	{
		var verts = new Assimp.Vector3D[2] { new Assimp.Vector3D( start.X, start.Y, start.Z ), new Assimp.Vector3D( end.X, end.Y, end.Z ) };
		missions.Enqueue( new DebugDrawMission
		{
			Vertices = verts,
			Color = color,
			Duration = duration > 0 ? duration : null
		} );
	}
}
