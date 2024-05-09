namespace Sour;

public sealed class Tilemap : Component
{
	protected override void OnAttached()
	{
		var size = 4f;
		for ( int z = 0; z < 16; z++ )
			for ( int x = 0; x < 16; x++ )
			{
				var cube = GameObject.Spawn();
				cube.AddComponent( new ModelComponent( "Resources/Models/Box/box.fbx" ) );
				cube.Transform.Position = Position + new Vector3( x * size, 0, z * size );
			}
	}
}
