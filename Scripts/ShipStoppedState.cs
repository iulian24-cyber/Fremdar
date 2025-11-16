using Godot;
using System;

public partial class ShipStoppedState : ShipState
{
	public static string StateName = "ShipStoppedState";
	
	public override string GetStateName()
	{
		return StateName;
	}
	
	public override void Process(double delta)
	{
		if (Godot.Input.IsActionJustPressed("E"))
			ShipStateMachine.Transition(ShipIdleState.StateName);
	}
	
	public override void PhysicsProcess(double delta)
	{
		Ship.Velocity = Ship._Gravity(delta);
	}
}
