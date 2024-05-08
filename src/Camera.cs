namespace Sour;

public class Camera : Component
{
    public static Camera Main;
    public float FieldOfView = 90;
    public Matrix4 ViewMatrix;
    public Matrix4 ProjectionMatrix;

    protected override void OnAttached()
    {
        Main = this;
        ViewMatrix = Matrix4.LookAt( Transform.Position, Vector3.Zero, Axis.Up );
        ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians( FieldOfView ),
            Engine.ScreenSize.X / (float)Engine.ScreenSize.Y,
            0.1f,
            300.0f
        );
    }

    public override void Render()
    {
        base.Render();
        ViewMatrix = Matrix4.LookAt( Transform.Position, Transform.Position + Transform.Forward, Axis.Up );

        ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians( FieldOfView ),
            Engine.ScreenSize.X / (float)Engine.ScreenSize.Y,
            0.1f,
            300.0f
        );
    }
}
