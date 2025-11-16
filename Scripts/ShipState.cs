using Godot;
using System;

[GlobalClass]
public partial class ShipState : State
{
	protected ShipController Ship;
	protected StateMachine ShipStateMachine;
	
	public void Init(ShipController ship)
	{
		Ship = ship;
		ShipStateMachine = Ship.ShipStateMachine;
	}
}
