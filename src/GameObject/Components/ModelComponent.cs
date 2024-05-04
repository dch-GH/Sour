using System.Diagnostics;

namespace Sour;

public class ModelComponent : Component
{
	public Vertex[] Vertices;
	public int VertexCount => Vertices.Length;
	public Material? Material;
	public Assimp.Vector3D[] Positions;
	public int IndexCount => Indices.Length;
	public uint[] Indices;
	public Assimp.Vector3D[] Normals;
	public Assimp.Vector3D[] UVs;

	public ModelComponent( string path, Material? material = null )
	{
		path = path.Trim();

		var ctx = new Assimp.AssimpContext();
		var imported = ctx.ImportFile( path, Assimp.PostProcessSteps.Triangulate );

		List<Assimp.Vector3D> positions = new();
		List<uint> indices = new();
		var mesh = imported.Meshes[0];

		var normals = mesh.Normals;
		if ( mesh.HasNormals )
			Normals = normals.ToArray();

		foreach ( var pos in mesh.Vertices )
			positions.Add( pos );

		foreach ( var i in mesh.GetUnsignedIndices() )
			indices.Add( i );

		// TODO: Check for other channels or whatever.
		if ( mesh.TextureCoordinateChannelCount > 0 )
		{
			UVs = mesh.TextureCoordinateChannels[0].ToArray();
		}

		Log.Info( mesh.VertexCount );

		Positions = positions.ToArray();
		Indices = indices.ToArray();

		Debug.Assert( Positions.Length == mesh.VertexCount );
		Debug.Assert( mesh.VertexCount == UVs.Length );

		var vertices = new List<Vertex>();
		for ( var i = 0; i < mesh.VertexCount; i++ )
		{
			vertices.Add( new Vertex
			{
				Position = Positions[i],
				Normal = Normals[i],
				UV = UVs[i]
			} );

		}


		Vertices = vertices.ToArray();

		// TODO: Material.Default (just the basic material to fallback to instead of making one here every time)
		if ( material is null )
		{
			Material = new Material( Material.DefaultVertexShaderPath, Material.DefaultFragmentShaderPath );
		}
		else
		{
			Material = material;
		}
	}

	public override void Render()
	{
		var r = Game.ModelRenderer;
		var job = new ModelDrawMission
		{
			Model = this,
			Matrix = GameObject.Transform.Matrix
		};
		r.PushModelMission( job );

		DebugDraw.Box3( GameObject.Bounds, Color4.Yellow, model: GameObject.Transform.Matrix );
	}
}
