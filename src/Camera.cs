namespace Sour;

public class Camera : Component
{
	public static Camera Main;
	public Matrix4 ViewMatrix;
	public Matrix4 ProjectionMatrix;
	public Box2 Bounds => _bounds;
	private Box2 _bounds;

	private float _orthoSize = 16;

	public Camera( float orthoSize = 16 )
	{
		_orthoSize = orthoSize;
	}

	public Vector3 ScreenToWorld( Vector2 screenPos )
	{
		var screenWidth = Engine.ScreenSize.X;
		var screenHeight = Engine.ScreenSize.Y;

		// Invert the view matrix
		var invertedViewMatrix = ViewMatrix.Inverted();

		// Convert screen position to NDC
		float ndcX = (2.0f * screenPos.X) / screenWidth - 1.0f;
		float ndcY = 1.0f - (2.0f * screenPos.Y) / screenHeight;

		// Transform from NDC to view space
		Vector3 viewPos = Vector3.TransformPosition( new Vector3( ndcX, ndcY, 0.0f ), invertedViewMatrix );
		return viewPos;
	}

	public Vector2 WorldToScreen( Vector3 worldPos )
	{
		var screenWidth = Engine.ScreenSize.X;
		var screenHeight = Engine.ScreenSize.Y;

		// Transform from world space to view space
		Vector3 viewPos = Vector3.TransformPosition( new Vector3( worldPos.X, worldPos.Y, 0.0f ), ViewMatrix );

		// If using a perspective projection, apply the projection matrix to get NDC
		Vector3 ndcPos = viewPos;
		// if ( projectionMatrix.M33 != 1.0f ) // Check if it's a perspective projection
		// {
		// 	ndcPos = Vector4.Transform( viewPos, projectionMatrix );
		// 	ndcPos /= ndcPos.W; // Perspective divide
		// }
		// else // Orthographic projection
		// {
		// 	ndcPos = viewPos;
		// }

		// Convert from NDC to screen space
		float screenX = (ndcPos.X + 1.0f) * 0.5f * screenWidth;
		float screenY = (1.0f - ndcPos.Y) * 0.5f * screenHeight;

		return new Vector2( screenX, screenY );
	}

	protected override void OnAttached()
	{
		UpdateViewBounds();
		Main = this;
	}

	public override void Render()
	{
		base.Render();
		UpdateViewBounds();
	}

	private void UpdateViewBounds()
	{
		var pos = Transform.Position.ToVector3();
		pos.Z = -5;

		ViewMatrix = Matrix4.LookAt( pos, pos + Transform.Forward, Axis.Up );

		ProjectionMatrix = Matrix4.CreateOrthographic(
			_orthoSize,
			_orthoSize,
			0.1f,
			1000.0f
		);

		var size = Engine.ScreenSize;
		var pos2d = Transform.Position;

		var x = size.X * 4;
		var y = size.Y * 4;
		_bounds = new Box2( -x, -y, size.X * 6, size.Y * 6 );
	}

	public bool ShouldRender( Vector3 pos )
	{
		var toScreen = WorldToScreen( pos );
		return _bounds.ContainsInclusive( toScreen );
	}
}
