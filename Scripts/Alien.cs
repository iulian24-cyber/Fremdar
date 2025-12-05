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
	HitArea3D HitCollisionArea;
	bool ChooseRandomAnim = false;
	int RandomNumber;
	private RandomNumberGenerator Rng = new RandomNumberGenerator();
	AudioStreamPlayer3D Hitting;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AlienAnimPlayer = GetNodeOrNull<AnimationPlayer>("Mesh&Animation/AnimationPlayer");
		InitialLocation = GetNodeOrNull<Node3D>("InitialLocation");
		InitialLocationPosition = InitialLocation.GlobalPosition;
		NavAgent = GetNodeOrNull<NavigationAgent3D>("NavigationAgent3D");
		HitCollisionArea = GetTree().CurrentScene.GetNodeOrNull<HitArea3D>("Cassian 4-62/Moveables/HitCollision/HitArea3D");
		Ship = GetTree().CurrentScene.GetNodeOrNull<ShipController>("Cassian 4-62");
		MartianArea = GetParent<MartianArea3D>();
		Hitting = GetNodeOrNull<AudioStreamPlayer3D>("AudioStreams/Hitting");
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
			else if (HitCollisionArea.AlienInHitCollision == true && HitCollisionArea.CurrentAlienInRange == this)
				AlienPhase = AlienPhases.AttackingShip;
		}
		if (AlienPhase == AlienPhases.ReturningInitialLocation)
		{
			if (MartianArea.ShipInsideMartianArea == true)
				AlienPhase = AlienPhases.FollowingShip;
			else if (NavAgent.IsNavigationFinished() == true)
				AlienPhase = AlienPhases.StayingInitialLocation;
		}
		if (AlienPhase == AlienPhases.AttackingShip)
		{
			if (HitCollisionArea.AlienInHitCollision == false)
			{
				AlienPhase = AlienPhases.FollowingShip;
				ChooseRandomAnim = false;
			}
			if (MartianArea.ShipInsideMartianArea == false)
			{
				AlienPhase = AlienPhases.ReturningInitialLocation;
				ChooseRandomAnim = false;
			}
		}
		//Changing Animations/Sounds/Logic using said Phases
		if (AlienPhase == AlienPhases.StayingInitialLocation) // Idle
		{
			AlienAnimPlayer.Play("Idle");
			AlienAnimPlayer.SpeedScale = 1f;
			Hitting.Stop();
		}
		else if (AlienPhase == AlienPhases.FollowingShip || AlienPhase == AlienPhases.ReturningInitialLocation) // Walk
		{
			AlienAnimPlayer.Play("Walk");
			AlienAnimPlayer.SpeedScale = 2f;
			Hitting.Stop();
		}
		else if (AlienPhase == AlienPhases.AttackingShip)
		{
			if (ChooseRandomAnim == false)
			{
				Rng.Randomize();
				RandomNumber = Rng.RandiRange(1, 2);
				ChooseRandomAnim = true;
			}
			if (RandomNumber == 1)
			{
				AlienAnimPlayer.SpeedScale = 1.3f;
				AlienAnimPlayer.Play("Kick");
			}
			else
			{
				AlienAnimPlayer.SpeedScale = 1f;
				AlienAnimPlayer.Play("Punch");
			}
			if (RandomNumber == 1 && AlienAnimPlayer.CurrentAnimationPosition >= 0.001f && AlienAnimPlayer.CurrentAnimationPosition <= 0.02f)
			{
				Ship.Health -= 5f;
				Hitting.SetStream(GD.Load<AudioStream>("res://Audio/SFX/kick.wav"));
				Hitting.SetVolumeDb(100f);
				Hitting.Play();
			}
			else if (RandomNumber == 2 && AlienAnimPlayer.CurrentAnimationPosition >= 0.01f && AlienAnimPlayer.CurrentAnimationPosition <= 0.02f)
			{
				Ship.Health -= 5f;
				Hitting.SetStream(GD.Load<AudioStream>("res://Audio/SFX/punch.mp3"));
				Hitting.SetVolumeDb(30f);
				Hitting.Play();
			}
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
			// test
			LookAt(new Vector3(Ship.GlobalPosition.X, GlobalPosition.Y, Ship.GlobalPosition.Z), Vector3.Up);
			NavAgent.SetTargetPosition(Ship.GlobalTransform.Origin);
			var NextNavPoint = NavAgent.GetNextPathPosition();
			Velocity = (NextNavPoint - GlobalTransform.Origin).Normalized() * Speed;
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
