using Godot;
using System;

public partial class RetreatState : Node, IState
{
public event EventHandler<IState,String> TransitionEvent;

	public void Enter()
	{
		GD.Print("Retreat Enter");
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
