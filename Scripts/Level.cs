using Godot;
using System;

public partial class Level : Node3D
{
	MeshInstance3D LoadingScreenMesh;
	ShipController Cassian;
	float Alpha = 1f;
	Label3D GPSLabel;
	Node3D Target;
	PackedScene Win, Menu;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GPSLabel = GetNodeOrNull<Label3D>("Cassian 4-62/Moveables/GPS/Label3D");
		Target = GetNodeOrNull<Node3D>("Sky&Environment/Sunrise&SunsetPoints/SunsetPoint");
		LoadingScreenMesh = GetNodeOrNull<MeshInstance3D>("Cassian 4-62/Moveables/LoadingScreen/MeshInstance3D");
		LoadingScreenMesh.Visible = true;
		Cassian = GetNodeOrNull<ShipController>("Cassian 4-62");
		Win = ResourceLoader.Load<PackedScene>("res://Scenes/win.tscn");
		Menu = ResourceLoader.Load<PackedScene>("res://Scenes/main_menu.tscn");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Mathf.Round(Cassian.GlobalPosition.DistanceTo(Target.GlobalPosition)) < 40f)
			GetTree().ChangeSceneToPacked(Win);
		if (Cassian.Health < 0f)
			GetTree().ChangeSceneToPacked(Menu);
		Material material = LoadingScreenMesh.GetActiveMaterial(0);
		BaseMaterial3D newmaterial;
		if (material is BaseMaterial3D)
		{
			newmaterial = (BaseMaterial3D)material;
			if (newmaterial.GetAlbedo() != new Color(0f, 0f, 0f, 0f))
			{
				if (Alpha > 0.95f)
					Alpha = Mathf.Lerp(Alpha, 0f, (float)delta * 0.05f);
				else if (Alpha > 0.7f)
					Alpha = Mathf.Lerp(Alpha, 0f, (float)delta * 0.2f);
				else if (Alpha > 0.4f)
					Alpha = Mathf.Lerp(Alpha, 0f, (float)delta * 1.5f);
				else if (Alpha > 0.01f)
					Alpha = Mathf.Lerp(Alpha, 0f, (float)delta * 7.5f);
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
		GPSLabel.Text = Mathf.Round(Cassian.GlobalPosition.DistanceTo(Target.GlobalPosition)) + "m";
	}
}
