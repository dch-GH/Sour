# Sour
### Simple game engine/renderer.

**Shader hot reloading** *wow!!*

https://github.com/dch-GH/Sour/assets/66768086/ea6ba996-efff-439b-8b8a-f36ae477d46c

**GameObjects**!!
```cs
		var cone = GameObject.Spawn();
		cone.AddComponent( new ModelComponent( "Resources/Models/Cone/cone.obj", new Material( fragShaderPath: "Resources/Shaders/frag.glsl" ) ) );
		cone.Transform.Position += Axis.Right * 3f;
		cone.AddComponent<RotatorComponent>();
```

**Components**!!!
```cs
class RotatorComponent : Component
{
	public override void Update()
	{
		base.Update();
		GameObject.Transform.Rotation = Quaternion.FromAxisAngle( Axis.Up + Axis.Forward * 0.5f, Time.Elapsed );
	}
}
```
