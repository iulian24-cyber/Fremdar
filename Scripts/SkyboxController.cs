using Godot;
using System;

public partial class SkyboxController : WorldEnvironment
{
	DirectionalLight3D Sun;
	Node3D A, B, C;
	Node3D Moon;
	float SunDegrees = 0.0f;
	float Distance = 0.0f;
	float SunEnergy = 1.0f;
	public override void _Ready()
	{
		Sun = GetParent()?.GetNodeOrNull<DirectionalLight3D>("Sun");
		Moon = GetParent()?.GetNodeOrNull<Node3D>("Moon");
		A = GetParent()?.GetNodeOrNull<Node3D>("Sunrise&SunsetPoints/SunrisePoint"); // Sunrise Point {-1}
		B = GetParent()?.GetNodeOrNull<Node3D>("Sunrise&SunsetPoints/SunsetPoint"); // Sunset Point {+1}
		C = GetParent()?.GetParent()?.GetNodeOrNull<CharacterBody3D>("Cassian 4-62"); // Ship Point
	}
	
	public override void _Process(double delta)
	{
		// Ship Distance To Sunrise & Sunset Points with Normalization from [0, 1] to [-1, 1] {2 * t - 1}
		if (C != null && B != null && A != null) {
			Distance = 2*((C.Position.X-A.Position.X)*(B.Position.X-A.Position.X)+(C.Position.Z-A.Position.Z)*(B.Position.Z-A.Position.Z))/((B.Position.X-A.Position.X)*(B.Position.X-A.Position.X)+(B.Position.Z-A.Position.Z)*(B.Position.Z-A.Position.Z))-1;
		}
		if (Sun != null)
		{
			SunDegrees = Distance * 180.0f;
			Transform3D T = Sun.Transform;
			T.Basis = new Basis(new Vector3(1, 0, 0), Mathf.DegToRad(SunDegrees));
			Sun.Transform = T;
			Moon.Transform = Sun.Transform.Inverse();
			if (Distance <= 0.0f)
			{
				Sun.LightEnergy = Mathf.Lerp(Sun.LightEnergy, 1.0f, 0.1f);
				Environment.FogLightEnergy = Mathf.Lerp(Environment.FogLightEnergy, 1.0f, 0.1f);
			}
			else
			{
				Sun.LightEnergy = Mathf.Lerp(Sun.LightEnergy, 0.0f, 0.1f);
				Environment.FogLightEnergy = Mathf.Lerp(Environment.FogLightEnergy, 0.0f, 0.1f);
			}
		}
		
		
		
		// Skybox Shader Parameters
		Vector3 sun_dir = GetNode<Node3D>("../Sun").GlobalTransform.Basis.Z;
		var moon_basis = GetNode<Node3D>("../Moon").GlobalTransform.Basis;
		Vector3 moon_dir = GetNode<Node3D>("../Moon").GlobalTransform.Basis.Z;

		var SkyMat = Environment?.Sky?.SkyMaterial as ShaderMaterial;
		SkyMat.SetShaderParameter("sun_dir", sun_dir);
		SkyMat.SetShaderParameter("moon_dir", moon_dir);
		SkyMat.SetShaderParameter("moon_to_world_to_object", moon_basis.Inverse());
	}
}
