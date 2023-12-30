using OpenTK.Mathematics;

namespace Sour;

internal class Model
{
	public Transform Transform = new();
	private Assimp.Vector3D[] importedVerts;
	private uint[] importedIndices;

	public Model( string path )
	{
		var ctx = new Assimp.AssimpContext();
		var imported = ctx.ImportFile( path, Assimp.PostProcessSteps.Triangulate );

		List<Assimp.Vector3D> verts = new();
		List<uint> indices = new();

		foreach ( var mesh in imported.Meshes )
		{
			foreach ( var v in mesh.Vertices )
				verts.Add( v );

			foreach ( var i in mesh.GetUnsignedIndices() )
				indices.Add( i );
		}

		importedVerts = verts.ToArray();
		importedIndices = indices.ToArray();
	}

	public void Render( Renderer r )
	{
		r.PushJob( new RenderJob
		{
			Vertices = importedVerts,
			Indices = importedIndices,
			ModelMatrix = Transform.Matrix,
			ShaderHandle = 0
		} );
	}
}
