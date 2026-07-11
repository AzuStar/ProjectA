using Godot;
using System;

public partial class PatrollingState : Node, IState
{
	public event EventHandler<IState,String> TransitionEvent;
	[Export] NavigationAgent3D navigationAgent;
	[Export] Node3D Route;
	[Export] CharacterBody3D Character;
	[Export] float CharacterSpeed;

	public void Enter()
	{
		//TransitionEvent?.Invoke(this,"ChasingState");
		Vector3 position = Vector3.Zero;
		position.X=Route.GlobalPosition.X;
		position.Z=Route.GlobalPosition.X;
		
		navigationAgent.TargetPosition = position;
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
		Vector3 destination = navigationAgent.GetNextPathPosition();
		Vector3 localDestination = destination - Character.GlobalPosition;
		Vector3 direction = localDestination.Normalized();

		Character.Velocity = direction * CharacterSpeed;
		Character.MoveAndSlide();

		return;
	}
}
