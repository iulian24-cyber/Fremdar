using Godot;
using System;

public partial class ShipMovingState : ShipState
{	
	public static string StateName = "ShipMovingState";
	
	public override string GetStateName()
	{
		return StateName;
	}
	
	public override void Process(double delta)
	{
		Vector2 ShipInput = Godot.Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown");
		if (ShipInput == Vector2.Zero)
			ShipStateMachine.Transition(ShipIdleState.StateName);
		if (Godot.Input.IsActionPressed("Shift"))
			ShipStateMachine.Transition(ShipAcceleratingState.StateName);
		if (Godot.Input.IsActionJustPressed("E"))
			ShipStateMachine.Transition(ShipStoppedState.StateName);	
	}
	
	public override void PhysicsProcess(double delta)
	{
		Ship.Velocity = Ship._Gravity(delta);
	}
}
