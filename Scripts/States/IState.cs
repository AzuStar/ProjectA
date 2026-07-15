using System;
using Godot;

public interface IState
{
    public event EventHandler<IState, String> TransitionEvent;
    public void Enter();
    public void Exit();
    public void StateUpdate(double _delta);
    public void StatePhysicsUpdate(double _delta);
}
