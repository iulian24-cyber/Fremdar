using Godot;
using System;

public partial class MainMenu : Node3D
{
	Label3D StartLabel;
	Label3D ExitLabel;
	AudioStreamPlayer Audio;
	PackedScene Level;
	public override void _Ready()
	{
		Level = ResourceLoader.Load<PackedScene>("res://Scenes/level.tscn");
		StartLabel = GetNodeOrNull<Label3D>("Buttons/Start/Label3D");
		ExitLabel = GetNodeOrNull<Label3D>("Buttons/Exit/Label3D");
		Audio = GetNodeOrNull<AudioStreamPlayer>("AudioStreamPlayer");
		Audio.Playing = true;
		Audio.Autoplay = true;
	}

	public override void _Process(double delta)
	{
	}
	
	private void StartEntered()
	{
		StartLabel.Modulate = new Color("0064b4");
	}
	
	private void StartExited()
	{
		StartLabel.Modulate = new Color(1.0f, 1.0f, 1.0f, 1.0f);
	}
	
	private void OnStartButtonPressed(Node camera, InputEvent @event, Vector3 position, Vector3 normal, int shapeIdx)
	{
		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed)
			{
				
				GetTree().ChangeSceneToPacked(Level);
			}
		}
	}
	
	private void ExitEntered()
	{
		ExitLabel.Modulate = new Color("0064b4");
	}
		
	private void ExitExited()
	{
		ExitLabel.Modulate = new Color(1.0f, 1.0f, 1.0f, 1.0f);
	}
	
	private void OnExitButtonPressed(Node camera, InputEvent @event, Vector3 position, Vector3 normal, int shapeIdx)
	{
		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed)
			{
				GetTree().Quit();
			}
		}
	}
}
