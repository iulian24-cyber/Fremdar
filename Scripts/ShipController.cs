using Godot;
using System;

[GlobalClass]
public partial class ShipController : CharacterBody3D
{
	[Export(PropertyHint.Range, "0.01,1.0,0.01")]
	float MouseSmoothness { get; set; } = 0.3f; // Lower is smoother
	[Export(PropertyHint.Range, "0.1,6.0,0.1")]
	float CameraSensitivity { get; set; } = 3.2f;
	[Export(PropertyHint.Range, "1.0,12.0,1.0")]
	float ChairSmoothness { get; set; } = 6.0f; // Lower is smoother
	bool MouseCaptured = false;
	Vector2 LookDir = Vector2.Zero;
	Vector2 TargetLookDir = Vector2.Zero;
	Vector2 SmoothedLookDir = Vector2.Zero;
	Camera3D Camera;
	Node3D Chair;
	public StateMachine ShipStateMachine;
	float Gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
	Vector3 GravityVelocity = Vector3.Zero;
	Vector2 MoveDir = Vector2.Zero;
	[Export] public float MovementSpeed = 10f;
	[Export] public float AccelerationSpeed = 15f;
	[Export] public float RotationSpeed = 0.03f;
	float CurrentSmoothRotation = 0.0f;
	float RotationLastInput = 0f;
	
	public override void _Ready()
	{
		CaptureMouse();
		Camera = GetNodeOrNull<Camera3D>("Camera");
		Chair = GetNodeOrNull<Node3D>("ShipMesh/Chair");
		ShipStateMachine = GetNodeOrNull<StateMachine>("StateMachine");
		ShipState[] States = new ShipState[]{
			new ShipIdleState(),
			new ShipMovingState(),
			new ShipStoppedState(),
			new ShipAcceleratingState(),
			new ShipAttackedState()
		};
		foreach (var state in States)
		{
			state.Init(this);
		}
		ShipStateMachine.StartMachine(States);
	}
	
	public override void _UnhandledInput(InputEvent @event)
	{
		if (MouseCaptured && @event is InputEventMouseMotion MotionEvent)
		{
			TargetLookDir = MotionEvent.Relative * 0.001f; // Save raw delta
		}
	}
	
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("Esc"))
		{
			MouseCaptured = !MouseCaptured;
			if (!MouseCaptured)
				ReleaseMouse();
			else
				CaptureMouse();
		}
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (MouseCaptured == true)
		{
			SmoothedLookDir = SmoothedLookDir.Lerp(TargetLookDir, MouseSmoothness);
			LookDir = SmoothedLookDir;
			if (Chair != null)
				RotateChair(delta);
			RotateCamera();	
			TargetLookDir = TargetLookDir.MoveToward(Vector2.Zero, MouseSmoothness);
			HandleJoypadCameraRotation(delta);
		}
		MoveAndSlide();
		//GD.Print(Velocity);
	}
	
	private void CaptureMouse()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		MouseCaptured = true;
	}

	private void ReleaseMouse()
	{
		Input.MouseMode = Input.MouseModeEnum.Visible;
		MouseCaptured = false;
	}
	
	private void RotateCamera(float SensMod = 1.0f)
	{
		Camera.Rotation = Camera.Rotation with { Y = Camera.Rotation.Y - LookDir.X * CameraSensitivity * SensMod };
		float NewPitch = Camera.Rotation.X - LookDir.Y * CameraSensitivity * SensMod;
		Camera.Rotation = Camera.Rotation with { X = Mathf.Clamp(NewPitch, Mathf.DegToRad(-89), Mathf.DegToRad(89)) };
		LookDir = Vector2.Zero;
	}
	
	private void RotateChair(double delta)
	{
		float CurrentY = Chair.Rotation.Y;
		float TargetY  = Camera.Rotation.Y;
		float Diff = Mathf.AngleDifference(CurrentY, TargetY);
		float AbsDiff = Mathf.Abs(Diff);
		float DynamicSpeed = ChairSmoothness;
		if (AbsDiff > Mathf.DegToRad(20f))
		{
			float Factor = Mathf.Clamp(AbsDiff / Mathf.DegToRad(80f), 1f, 4f);
			DynamicSpeed *= Factor;
		}
		float NewY = Mathf.LerpAngle(CurrentY, TargetY, (float)delta * DynamicSpeed);
		Chair.Rotation = Chair.Rotation with { Y = NewY };
	}
	
	private void HandleJoypadCameraRotation(double delta, float SensMod = 1.0f)
	{
		Vector2 JoypadDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		if (JoypadDir.Length() > 0)
		{
			LookDir += JoypadDir * (float)delta;
			RotateCamera(SensMod);
			LookDir = Vector2.Zero;
		}
	}
	
	public void _Rotation(double delta)
	{
		float TurnInput = 0f;
		if (Input.IsActionPressed("MoveLeft")) TurnInput = 1f;
		if (Input.IsActionPressed("MoveRight")) TurnInput = -1f;
		if (TurnInput != 0f && Input.IsActionPressed("MoveLeft") != Input.IsActionPressed("MoveRight"))
		{
			if ((Input.IsActionJustPressed("MoveLeft") || Input.IsActionJustPressed("MoveRight")))
				CurrentSmoothRotation = 0.25f * CurrentSmoothRotation;
			else
			{
				Rotation = Rotation with { Y = Rotation.Y + TurnInput * CurrentSmoothRotation };
				CurrentSmoothRotation = Mathf.Lerp(CurrentSmoothRotation, RotationSpeed, (float)delta * 0.8f);
				RotationLastInput = TurnInput;
				if (RotationSpeed - CurrentSmoothRotation < 0.001f)
					CurrentSmoothRotation = RotationSpeed;
			}
		}
		else
		{
			Rotation = Rotation with { Y = Rotation.Y + RotationLastInput * CurrentSmoothRotation };
			CurrentSmoothRotation = Mathf.Lerp(CurrentSmoothRotation, 0.0f, (float)delta * 12f);
			if (CurrentSmoothRotation < 0.001f)
				CurrentSmoothRotation = 0.0f;
		}
	}
	
	public Vector3 _Gravity(double delta)
	{
		if (IsOnFloor())
			GravityVelocity = Vector3.Zero;
		else
		{
			Vector3 Target = new Vector3(0, Velocity.Y - Gravity, 0);
			GravityVelocity = GravityVelocity.MoveToward(Target, Gravity * (float)delta);
		}
		return GravityVelocity;
	}
	
	public Vector3 _Move(double delta)
	{
		Vector3 Vel = Velocity;
		Vector2 MoveVec = Input.GetVector(
			"MoveRight",       // negative X
			"MoveLeft",      // positive X
			"MoveDown",    // positive Y
			"MoveUp"   // negative Y
		);
		Vector3 Forward = -GlobalTransform.Basis.Z;
		Forward.Y = 0;
		Forward = Forward.Normalized();
		if (MoveVec.Y != 0f)
		{
			Vel.X = Forward.X * MoveVec.Y * MovementSpeed;
			Vel.Z = Forward.Z * MoveVec.Y * MovementSpeed;
		}
		else
		{
			// Smooth stop
			Vel.X = Mathf.Lerp(Vel.X, 0f, (float)delta * 7f);
			Vel.Z = Mathf.Lerp(Vel.Z, 0f, (float)delta * 7f);
		}
		return Vel;
	}
	
	public Vector3 _Accelerate(double delta)
	{
		Vector3 Vel = Velocity;
		Vector2 MoveVec = Input.GetVector(
			"MoveRight",       // negative X
			"MoveLeft",      // positive X
			"MoveDown",    // positive Y
			"MoveUp"   // negative Y
		);
		Vector3 Forward = -GlobalTransform.Basis.Z;
		Forward.Y = 0;
		Forward = Forward.Normalized();
		if (MoveVec.Y != 0f)
		{
			Vel.X = Forward.X * MoveVec.Y * AccelerationSpeed;
			Vel.Z = Forward.Z * MoveVec.Y * AccelerationSpeed;
		}
		else
		{
			// Smooth stop
			Vel.X = Mathf.Lerp(Vel.X, 0f, (float)delta * 7f);
			Vel.Z = Mathf.Lerp(Vel.Z, 0f, (float)delta * 7f);
		}
		return Vel;
	}
}
