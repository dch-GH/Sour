using VelcroPhysics;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Templates;

namespace Sour;

public sealed class PhysicsComponent : Component
{
	/// <summary>
	/// Has this been added to the CollisionComponent world?
	/// </summary>
	public bool Initialized;
	public FixtureTemplate FixtureDef;
	public BodyTemplate BodyDef;

	public Body Body;

	/// <summary>
	/// Right now being used for driving AI characters.
	/// </summary>
	public Vector2 ImpulseVelocity;

	protected override void OnAttached()
	{
		base.OnAttached();

		var pos = new Microsoft.Xna.Framework.Vector2( Transform.Position.X, Transform.Position.Y );
		Body = VelcroPhysics.Factories.BodyFactory.CreateBody( Engine.Physics.Simulation, pos, Transform.Rotation, BodyDef.Type, null );
	}

	public override void Update()
	{
		// Body.GetTransform( out var xform );
		GameObject.Transform.Position = Body.Position.FromXna();
		GameObject.Transform.Rotation = Body.Rotation;
	}

	// public override void PhysicsUpdate()
	// {
	// 	base.PhysicsUpdate();

	// 	// Body.GetTransform( out var xform );
	// 	GameObject.Transform.Position = Body.Position.FromXna();
	// 	GameObject.Transform.Rotation = Body.Rotation;
	// }
}
