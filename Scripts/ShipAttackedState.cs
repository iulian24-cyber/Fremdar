using Godot;
using System;

public partial class ShipAttackedState : ShipState
{
	public static string StateName = "ShipAttackedState";
	
	public override string GetStateName()
	{
		return StateName;
	}
	
	public override void Process(double delta)
	{

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
