using Godot;
using System;

[Tool]
public partial class SkyboxController : WorldEnvironment
{
	public override void _Process(double delta)
	{
		Vector3 sun_dir = GetNode<Node3D>("../Sun").GlobalTransform.Basis.Z;
		var moon_basis = GetNode<Node3D>("../Moon").GlobalTransform.Basis;
		Vector3 moon_dir = GetNode<Node3D>("../Moon").GlobalTransform.Basis.Z;

		var skyMat = Environment?.Sky?.SkyMaterial as ShaderMaterial;
		skyMat.SetShaderParameter("sun_dir", sun_dir);
		skyMat.SetShaderParameter("moon_dir", moon_dir);
		skyMat.SetShaderParameter("moon_to_world_to_object", moon_basis.Inverse());
	}
}
