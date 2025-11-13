using Godot;
using System;

public partial class State : Resource
{
	// Called when the machine transitions to this state
	public virtual void Enter()
	{
	}

	// Called when this state ends
	public virtual void Exit()
	{
	}

	// Called when there's some kind of user input
	public virtual void Input(InputEvent @event)
	{
	}

	// Called in Godot's main update cycle
	public virtual void Process(double delta)
	{
	}

	// Called in Godot's main physics update cycle
	public virtual void PhysicsProcess(double delta)
	{
	}

	public virtual string GetStateName()
	{
		GD.PushError("Method GetStateName() must be defined.");
		return "";
	}
}
