using Godot;
using System;

public partial class Level : Node3D
{
	MeshInstance3D LoadingScreenMesh;
	ShipController Cassian;
	float Alpha = 1f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		LoadingScreenMesh = GetNodeOrNull<MeshInstance3D>("Cassian 4-62/Moveables/LoadingScreen/MeshInstance3D");
		Cassian = GetNodeOrNull<ShipController>("Cassian 4-62");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Material material = LoadingScreenMesh.GetActiveMaterial(0);
		BaseMaterial3D newmaterial;
		if (material is BaseMaterial3D)
		{
			newmaterial = (BaseMaterial3D)material;
			if (newmaterial.GetAlbedo() != new Color(0f, 0f, 0f, 0f))
			{
				if (Alpha > 0.95f)
					Alpha = Mathf.Lerp(Alpha, 0f, (float)delta * 0.1f);
				else if (Alpha > 0.7f)
					Alpha = Mathf.Lerp(Alpha, 0f, (float)delta * 0.3f);
				else if (Alpha > 0.4f)
					Alpha = Mathf.Lerp(Alpha, 0f, (float)delta * 1.5f);
				else if (Alpha > 0.01f)
					Alpha = Mathf.Lerp(Alpha, 0f, (float)delta * 2.5f);
				else
					Alpha = 0f;
				newmaterial.SetAlbedo(new Color(0f, 0f, 0f, Alpha));
			}
			else if(LoadingScreenMesh.Visible == true)
			{
				Cassian.ShipStateMachine.Transition(ShipIdleState.StateName);
				LoadingScreenMesh.Visible = false;
			}
		}
	}
}
