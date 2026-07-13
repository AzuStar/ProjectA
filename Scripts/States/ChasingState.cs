using System;
using Godot;

public partial class ChasingState : Node, IState
{
    public event EventHandler<IState, String> TransitionEvent;

    [Export]
    NavigationAgent3D navigationAgent;

    float CharacterSpeed;
    CharacterBody3D Character;
    Vector3 currentGoal;
    Node3D goal;

    public void SetCharacterSpeed(float value) => CharacterSpeed = value;

    public void SetCharacter(CharacterBody3D value) => Character = value;

    public void SetGoal(Node3D _goal)
    {
        if (goal == null)
            goal = _goal;

        currentGoal = Vector3.Zero;
        currentGoal.X = goal.GlobalPosition.X;
        currentGoal.Z = goal.GlobalPosition.Z;

        navigationAgent.TargetPosition = currentGoal;
    }

    public void Enter()
    {
        if (goal == null)
            return;

        currentGoal = Vector3.Zero;
        currentGoal.X = goal.GlobalPosition.X;
        currentGoal.Z = goal.GlobalPosition.Z;

        navigationAgent.TargetPosition = currentGoal;
    }

    public void Exit()
    {
        goal = null;
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

        if (currentPosition.DistanceTo(currentGoal) <= 0.1)
        {
            TransitionEvent?.Invoke(this, "SearchState");
            return;
        }

        Vector3 destination = navigationAgent.GetNextPathPosition();
        Vector3 localDestination = destination - Character.GlobalPosition;
        Vector3 direction = localDestination.Normalized();

        Character.Velocity = direction * CharacterSpeed;
        Character.LookAt(destination);
        Character.MoveAndSlide();
    }
}
