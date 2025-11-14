using Godot;
using System;

[GlobalClass]
public partial class ShipState : State
{
	ShipController Ship;
	StateMachine ShipStateMachine;
	
	public void Init(ShipController ship)
	{
		Ship = ship;
		ShipStateMachine = Ship.stateMachine;
	}
}
