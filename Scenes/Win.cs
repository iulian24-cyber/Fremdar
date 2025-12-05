using Godot;
using System;

public partial class Win : Node3D
{
	PackedScene Menu;
	float time = 150f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Visible;
		Menu = ResourceLoader.Load<PackedScene>("res://Scenes/main_menu.tscn");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (time < 0f)
			GetTree().ChangeSceneToPacked(Menu);
		time--;
	}
}
