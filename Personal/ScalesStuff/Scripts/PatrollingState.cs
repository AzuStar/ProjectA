using Godot;
using System;

public partial class PatrollingState : Node, IState
{
	public event EventHandler<IState,String> TransitionEvent;
	public void Enter()
	{
		TransitionEvent?.Invoke(this,"ChasingState");
		return;
	}
	public void Exit()
	{
		return;
	}
	public void StateUpdate(double _delta)
	{
		return;
	}
	public void StatePhysicsUpdate(double _delta)
	{
		return;
	}
}
