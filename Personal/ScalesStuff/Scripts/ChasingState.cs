using Godot;
using System;

public partial class ChasingState : Node, IState
{
	public event EventHandler<IState,String> TransitionEvent;	
	
	[Export] NavigationAgent3D navigationAgent;
	[Export] CharacterBody3D Character;
	[Export] float CharacterSpeed;

	Vector3 currentGoal;

	Node3D goal;
	public void SetGoal(Node3D _goal) => goal = _goal;
	
	public void Enter()
	{
		GD.Print("Chase");
		if (goal == null) TransitionEvent?.Invoke(this,"RetreatState");

		currentGoal = Vector3.Zero;
		currentGoal.X=goal.GlobalPosition.X;
		currentGoal.Z=goal.GlobalPosition.Z;

		navigationAgent.TargetPosition = currentGoal;
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
		Vector3 currentPosition = Vector3.Zero;
		currentPosition.X = Character.GlobalPosition.X;
		currentPosition.Z = Character.GlobalPosition.Z;
		if(currentPosition.DistanceTo(currentGoal)<=0.1)
		{
			GD.Print("Goal?");
		}

		Vector3 destination = navigationAgent.GetNextPathPosition();
		Vector3 localDestination = destination - Character.GlobalPosition;
		Vector3 direction = localDestination.Normalized();

		Character.Velocity = direction * CharacterSpeed;
		Character.LookAt(destination);
		Character.MoveAndSlide();

	}

}
