using OpenTK.Windowing.Common;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Templates;

namespace Sour;

public sealed class PhysicsSystem
{
	public World Simulation => _simulation;
	private World _simulation;

	internal PhysicsSystem( Vector2 gravity )
	{
		_simulation = new( gravity.ToXna() );
		Engine.UpdateEmitter.OnUpdateStage += Update;
	}

	private void Update( UpdateStage stage, FrameEventArgs args )
	{
		if ( stage is not UpdateStage.PreRender )
			return;

		_simulation.Step( Time.Delta );
	}

	public static PhysicsComponent CreatePhysics( BodyType bodyType, FixtureTemplate fixture, bool allowRotation = false )
	{
		var phys = new PhysicsComponent()
		{
			BodyDef = new BodyTemplate() { Type = bodyType, AllowRotation = allowRotation },
			FixtureDef = fixture,
		};

		return phys;
	}
}
