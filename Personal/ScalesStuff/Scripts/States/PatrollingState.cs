using Godot;
using System;

public partial class PatrollingState : Node, IState
{
	public event EventHandler<IState,String> TransitionEvent;
	[Export] NavigationAgent3D navigationAgent;
	[Export] Node3D[] Route;
	[Export] CharacterBody3D Character;
	[Export] float CharacterSpeed;
	
	Vector3 currentGoal;
	int currentIndex=0;
	public void Enter()
	{
		//TransitionEvent?.Invoke(this,"ChasingState");
		GD.Print("Patroll");
		currentGoal = Vector3.Zero;
		currentGoal.X=Route[currentIndex].GlobalPosition.X;
		currentGoal.Z=Route[currentIndex].GlobalPosition.Z;
		
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
		

		if(currentPosition.DistanceTo(currentGoal)<=0.5)
		{
			currentIndex++;

			if (currentIndex>=Route.Length){ 
				currentIndex = 0;
			}
			
			currentGoal = Vector3.Zero;
			currentGoal.X = Route[currentIndex].GlobalPosition.X;
			currentGoal.Z = Route[currentIndex].GlobalPosition.Z;
		
			navigationAgent.TargetPosition = currentGoal;

			return;	
		}

		Vector3 destination = navigationAgent.GetNextPathPosition();
		Vector3 localDestination = destination - Character.GlobalPosition;
		Vector3 direction = localDestination.Normalized();

		Character.Velocity = direction * CharacterSpeed;
		Character.LookAt(destination);
		Character.MoveAndSlide();

		return;
	}

	public void ChaseTime() => TransitionEvent?.Invoke(this,"ChasingState");
}
