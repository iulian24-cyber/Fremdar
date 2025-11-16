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
		if (Godot.Input.IsActionJustPressed("E"))
			ShipStateMachine.Transition(ShipStoppedState.StateName);
	}
	
	public override void PhysicsProcess(double delta)
	{
		Ship.Velocity = Ship._Gravity(delta);
	}
}
