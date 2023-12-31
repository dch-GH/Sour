namespace Sour;

public class Model
{
	public Transform Transform = new();
	public Shader? Shader;
	public Assimp.Vector3D[] Vertices;
	public uint[] Indices;

	public Model( string path, string? vtxShaderPath = null, string? fragShaderPath = null )
	{
		var ctx = new Assimp.AssimpContext();
		var imported = ctx.ImportFile( path, Assimp.PostProcessSteps.Triangulate );

		List<Assimp.Vector3D> verts = new();
		List<uint> indices = new();
		foreach ( var mesh in imported.Meshes )
		{
			var normals = mesh.Normals;
			var vertIndex = 0;
			foreach ( var v in mesh.Vertices )
			{
				verts.Add( v );
				if ( mesh.HasNormals )
					verts.Add( normals[vertIndex] );

				vertIndex += 1;
			}

			foreach ( var i in mesh.GetUnsignedIndices() )
				indices.Add( i );
		}

		Vertices = verts.ToArray();
		Indices = indices.ToArray();

		vtxShaderPath ??= Shader.DefaultVertexShaderPath;
		fragShaderPath ??= Shader.DefaultFragmentShaderPath;

		Shader = new Shader( vtxShaderPath, fragShaderPath );
	}

	public void Render( Renderer r )
	{
		var job = new RenderJob
		{
			Model = this,
			Matrix = Transform.Matrix
		};
		r.PushJob( job );
	}
}
