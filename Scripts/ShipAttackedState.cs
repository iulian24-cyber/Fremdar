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
		Ship.Velocity = Ship._Gravity(delta);
	}
}
