using Godot;
using System;

public partial class ShipIdleState : ShipState
{
	public static string StateName = "ShipIdleState";
	
	public override string GetStateName()
	{
		return StateName;
	}
	
	public override void Process(double delta)
	{
		Vector2 ShipInput = Godot.Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown");
		if (ShipInput != Vector2.Zero)
		{
			if (Godot.Input.IsActionPressed("Shift"))
				ShipStateMachine.Transition(ShipAcceleratingState.StateName);
			else
				ShipStateMachine.Transition(ShipMovingState.StateName);
		}
		if (Ship.ShipEngine.PitchScale > 1f)
			Ship.ShipEngine.PitchScale -= (float)delta;
	}
	
	public override void PhysicsProcess(double delta)
	{
		Vector3 TargetVelocity = Ship._Gravity(delta);
		if (Ship.Velocity != TargetVelocity)
		{
			if ((TargetVelocity - Ship.Velocity).Length() > 0.01f)
				Ship.Velocity = Ship.Velocity.Lerp(TargetVelocity, 0.1f);
			else
				Ship.Velocity = TargetVelocity;
		}
		Ship._Rotation(delta);
		Ship._BodyRotation(delta);
	}
}
