using Godot;
using System;

[GlobalClass]
public partial class HitArea3D : Area3D
{
	public Alien CurrentAlienInRange = null;
	public bool AlienInHitCollision = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		BodyExited += OnBodyExited;
	}
	
	private void OnBodyEntered(Node3D Body)
	{
		if (Body is Alien)
			AlienInHitCollision = true;
		if (Body is Alien alien)
			CurrentAlienInRange = alien;
	}
	
	private void OnBodyExited(Node3D Body)
	{
		if (Body is Alien)
			AlienInHitCollision = false;
		if (Body is Alien alien && CurrentAlienInRange == alien)
			CurrentAlienInRange = null;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
