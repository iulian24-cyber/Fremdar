using Godot;
using System;

public partial class ShipController : CharacterBody3D
{
	[Export(PropertyHint.Range, "0.01,1.0,0.01")]
	float MouseSmoothness { get; set; } = 0.3f; // Lower is smoother
	[Export(PropertyHint.Range, "0.1,6.0,0.1")]
	float CameraSensitivity { get; set; } = 3.2f;
	bool MouseCaptured = false;
	Vector2 LookDir = Vector2.Zero;
	Vector2 TargetLookDir = Vector2.Zero;
	Vector2 SmoothedLookDir = Vector2.Zero;
	Camera3D Camera;
	Node3D Chair;
	
	public override void _Ready()
	{
		CaptureMouse();
		Camera = GetNodeOrNull<Camera3D>("ShipMesh/Chair/Camera");
		Chair = GetNodeOrNull<Node3D>("ShipMesh/Chair");
		if (Chair != null)
		{

		}
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
		if (Input.IsActionJustPressed("esc"))
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
			{

			}
			RotateCamera();	
			TargetLookDir = TargetLookDir.MoveToward(Vector2.Zero, MouseSmoothness);
			HandleJoypadCameraRotation(delta);
		}
		
		MoveAndSlide();
	}
	
	public void CaptureMouse()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		MouseCaptured = true;
	}

	public void ReleaseMouse()
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
}
