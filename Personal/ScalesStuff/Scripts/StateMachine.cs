using Godot;
using System;
using System.Collections.Generic;

public partial class StateMachine : Node
{
	Dictionary<string,IState> states = new Dictionary<string,IState>();
	public IState currentState;
	[Export]
	public Node startingState;

	public override void _Ready()
	{
		//populate the dictionary with the children
		foreach (GodotObject child in GetChildren())
		{
		    if (child is IState state)
				{
					states[((child as Node).Name).ToString().ToLower()]=state;	
					state.TransitionEvent += OnStateTransition;
				}
		}
		
		//setup and check the starting current state
		if (states[startingState.Name.ToString().ToLower()]!=null)
		{
			currentState = states[startingState.Name.ToString().ToLower()];
			currentState.Enter();
		}

	}

	public override void _Process(double delta)
	{
		if (currentState != null) currentState.StateUpdate(delta);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (currentState != null) currentState.StatePhysicsUpdate(delta);
	}

	void OnStateTransition(IState state, String newStateName)
	{
		if (state != currentState) return;

		IState newState = states[newStateName.ToString().ToLower()];
		if (newState == null) return;

		if (currentState != null) currentState.Exit();
		newState.Enter();
		currentState=newState;

	}
}
