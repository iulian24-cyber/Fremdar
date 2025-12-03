using Godot;
using System;

[GlobalClass]
public partial class MartianArea3D : Area3D
{
	public bool ShipInsideMartianArea = false;
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		BodyExited += OnBodyExited;
	}

	private void OnBodyEntered(Node3D Body)
	{
		if (Body is ShipController)
			ShipInsideMartianArea = true;
	}
	
	private void OnBodyExited(Node3D Body)
	{
		if (Body is ShipController)
			ShipInsideMartianArea = false;
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
