namespace Sour;

public static class BoxExtensions
{
	public static Box2 ToBox2( this Box3 self )
	{
		var min = new Vector2( self.Min.X, self.Min.Y );
		var max = new Vector2( self.Max.X, self.Max.Y );
		return new Box2( min, max );
	}
}
