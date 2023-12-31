﻿namespace Sour;

public class Model
{
	public Transform Transform = new();
	public Material? Material;
	public Assimp.Vector3D[] Vertices;
	public uint[] Indices;
	public Assimp.Vector3D[] Normals;

	public Model( string path, string? vtxShaderPath = null, string? fragShaderPath = null )
	{
		var ctx = new Assimp.AssimpContext();
		var imported = ctx.ImportFile( path, Assimp.PostProcessSteps.Triangulate );

		List<Assimp.Vector3D> verts = new();
		List<uint> indices = new();
		foreach ( var mesh in imported.Meshes )
		{
			var normals = mesh.Normals;
			if ( mesh.HasNormals )
				Normals = normals.ToArray();

			var vertIndex = 0;
			foreach ( var v in mesh.Vertices )
			{
				verts.Add( v );
				if ( mesh.HasNormals )
				{
					verts.Add( normals[vertIndex] );
				}

				vertIndex += 1;
			}

			foreach ( var i in mesh.GetUnsignedIndices() )
				indices.Add( i );
		}

		Vertices = verts.ToArray();
		Indices = indices.ToArray();

		vtxShaderPath ??= Material.DefaultVertexShaderPath;
		fragShaderPath ??= Material.DefaultFragmentShaderPath;

		Material = new Material( vtxShaderPath, fragShaderPath );
	}

	public void Render( ModelRenderer r )
	{
		var job = new ModelDrawMission
		{
			Model = this,
			Matrix = Transform.Matrix
		};
		r.PushModelMission( job );
	}
}
