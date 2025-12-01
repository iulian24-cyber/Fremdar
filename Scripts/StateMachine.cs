using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class StateMachine : Node
{
	[Export] public bool IsLogEnabled { get; set; } = false;

	public bool IsRunning { get; private set; } = false;
	public bool PlayerInCrouchingArea { get; private set; } = false;

	public State _currentState;
	private readonly Dictionary<string, State> _states = new();
	private string _parentNodeName = "";

	// Start this state machine
	public void StartMachine(State[] initStates)
	{
		_parentNodeName = GetParent()?.Name ?? "Unknown";

		foreach (var state in initStates)
		{
			_states[state.GetStateName()] = state;
		}

		_currentState = initStates.Length > 0 ? initStates[0] : null;

		if (_currentState == null)
		{
			GD.PushError("StateMachine started with no initial state!");
			return;
		}

		if (IsLogEnabled)
			GD.Print($"[{_parentNodeName}]: Entering state \"{_currentState.GetStateName()}\"");

		_currentState.Enter();
		IsRunning = true;
	}

	public override void _Input(InputEvent @event)
	{
		_currentState?.Input(@event);
	}

	public override void _Process(double delta)
	{
		_currentState?.Process(delta);
	}

	public override void _PhysicsProcess(double delta)
	{
		_currentState?.PhysicsProcess(delta);
	}

	// Attempt a transition to the new state
	public void Transition(string newStateName)
	{
		if (!_states.TryGetValue(newStateName, out var newState))
		{
			GD.PushError($"An attempt has been made to transition to a non-existent state ({newStateName}).");
			return;
		}

		var currentStateName = _currentState?.GetStateName();

		if (newState == _currentState)
		{
			GD.PushWarning("An attempt to transition to the current state has been made. Ignoring request.");
			return;
		}

		if (IsLogEnabled)
			GD.Print($"[{_parentNodeName}]: Exiting state \"{currentStateName}\"");

		_currentState?.Exit();
		_currentState = newState;

		if (IsLogEnabled)
			GD.Print($"[{_parentNodeName}]: Entering state \"{_currentState.GetStateName()}\"");

		_currentState.Enter();
	}
}
