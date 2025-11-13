using Godot;
using System;

public partial class ShipPoint : Node3D
{
	[Export] private float Speed = 5.0f;
	private Vector3 direction = Vector3.Zero;

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		// Update direction (example: move along X axis)
		direction.X += 1.0f * (float)delta;

		// Calculate velocity
		Vector3 velocity = new Vector3(
			direction.X * Speed,
			direction.Y * Speed,
			direction.Z * Speed
		);

		// Move the node manually
		Position += velocity * (float)delta * 0.02f;
	}
}
