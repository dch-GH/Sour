namespace Sour;

public static class VectorExtensions
{
	public static Vector3 ToVector3( this Vector2 self )
	{
		return new Vector3( self.X, self.Y, 0 );
	}

	public static Vector2 ToVector2( this Vector3 self )
	{
		return new Vector2( self.X, self.Y );
	}

	public static Microsoft.Xna.Framework.Vector2 ToXna( this Vector2 self )
	{
		return new Microsoft.Xna.Framework.Vector2( self.X, self.Y );
	}

	public static Vector2 FromXna( this Microsoft.Xna.Framework.Vector2 self )
	{
		return new Vector2( self.X, self.Y );
	}
}
