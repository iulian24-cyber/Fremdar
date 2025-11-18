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
		Vector3 TargetVelocity = Ship._Gravity(delta);
		if ((TargetVelocity - Ship.Velocity).Length() > 0.01f)
			Ship.Velocity = Ship.Velocity.Lerp(TargetVelocity, 0.1f);
		else
			Ship.Velocity = TargetVelocity;
	}
}
