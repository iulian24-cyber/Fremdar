using Godot;
using System;

[GlobalClass]
public partial class Alien : CharacterBody3D
{
	float Speed = 6f;
	enum AlienPhases {
		StayingInitialLocation, // Waiting for ship to enter MartianArea3D
		FollowingShip, // Following ship until is seen or is in ship area3d
		AttackingShip, // Attacking if in ship area3d until seen
		ReturningInitialLocation // If seen or if ship leaving MartianArea3D returning to point A
	}
	AlienPhases AlienPhase;
	AnimationPlayer AlienAnimPlayer;
	// InitialLocation gets global position where alien is put in Editor when _Ready() function runs
	Node3D InitialLocation;
	Vector3 InitialLocationPosition;
	MartianArea3D MartianArea;
	ShipController Ship;
	NavigationAgent3D NavAgent;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AlienAnimPlayer = GetNodeOrNull<AnimationPlayer>("Mesh&Animation/AnimationPlayer");
		InitialLocation = GetNodeOrNull<Node3D>("InitialLocation");
		InitialLocationPosition = InitialLocation.GlobalPosition;
		NavAgent = GetNodeOrNull<NavigationAgent3D>("NavigationAgent3D");
		Ship = GetTree().CurrentScene.GetNodeOrNull<ShipController>("Cassian 4-62");
		MartianArea = GetParent<MartianArea3D>();
		AlienPhase = AlienPhases.StayingInitialLocation;
		AlienAnimPlayer.Play("Idle");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//Changing Phases
		if (AlienPhase == AlienPhases.StayingInitialLocation && MartianArea.ShipInsideMartianArea == true)
			AlienPhase = AlienPhases.FollowingShip;
		if (AlienPhase == AlienPhases.FollowingShip)
		{ 
			if (MartianArea.ShipInsideMartianArea == false)
				AlienPhase = AlienPhases.ReturningInitialLocation;
			//else if (MartianArea.Ship == true) // in proximity
				//AlienPhase = Aline.AttackingShip;
		}
		if (AlienPhase == AlienPhases.ReturningInitialLocation)
		{
			if (MartianArea.ShipInsideMartianArea == true)
				AlienPhase = AlienPhases.FollowingShip;
			else if (NavAgent.IsNavigationFinished() == true)
				AlienPhase = AlienPhases.StayingInitialLocation;
		}
		//Changing Animations using said Phases
		if (AlienPhase == AlienPhases.StayingInitialLocation) // Idle
		{
			AlienAnimPlayer.Play("Idle");
			AlienAnimPlayer.SpeedScale = 1f;
		}
		else if (AlienPhase == AlienPhases.FollowingShip || AlienPhase == AlienPhases.ReturningInitialLocation) // Walk
		{
			AlienAnimPlayer.Play("Walk");
			AlienAnimPlayer.SpeedScale = 2f;
		}
		else if (AlienPhase == AlienPhases.AttackingShip) // Kick/Punch/Punch1 (insert randomizer)
		{
			AlienAnimPlayer.Play("Kick");
			AlienAnimPlayer.SpeedScale = 1f;
		}
	}
	
	public override void _PhysicsProcess(double delta)
	{
		//Changing Physics using said Phases
		if (AlienPhase == AlienPhases.FollowingShip)
		{	
			LookAt(new Vector3(Ship.GlobalPosition.X, GlobalPosition.Y, Ship.GlobalPosition.Z), Vector3.Up);
			NavAgent.SetTargetPosition(Ship.GlobalTransform.Origin);
			var NextNavPoint = NavAgent.GetNextPathPosition();
			Velocity = (NextNavPoint - GlobalTransform.Origin).Normalized() * Speed;
		}
		else if (AlienPhase == AlienPhases.AttackingShip)
		{
			
		}
		else if (AlienPhase == AlienPhases.ReturningInitialLocation)
		{
			LookAt(InitialLocationPosition, Vector3.Up);
			NavAgent.SetTargetPosition(InitialLocationPosition);
			var NextNavPoint = NavAgent.GetNextPathPosition();
			Velocity = (NextNavPoint - GlobalTransform.Origin).Normalized() * Speed;
		}
		else
			Velocity = Vector3.Zero;
		MoveAndSlide();
	}
}
