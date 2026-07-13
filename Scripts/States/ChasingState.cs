using System;
using Godot;
using ProjectA.Game.Enemies;

public partial class ChasingState : Node, IState
{
    public event EventHandler<IState, String> TransitionEvent;

    [Export]
    NavigationAgent3D navigationAgent;

    float CharacterSpeed;
    CharacterBody3D Character;
    IEnemyAnimationController animationController;
    Vector3 currentGoal;

    [Export]
    public float chaseGoalStopThreshold = 0.1f;

    [Export]
    public float turnSpeedDegreesPerSecond = 360.0f;

    [Export]
    public float stuckDisengageTime = 1.5f;

    [Export]
    public float stuckDistanceThreshold = 0.02f;

    float stuckTimer;
    Vector3 previousPosition;

    public void SetCharacterSpeed(float value) => CharacterSpeed = value;

    public void SetCharacter(CharacterBody3D value) => Character = value;

    public void SetAnimationController(IEnemyAnimationController value) => animationController = value;

    public void SetGoal(Vector3 goalPosition)
    {
        currentGoal = goalPosition;
        currentGoal.Y = 0.0f;

        navigationAgent.TargetPosition = currentGoal;
    }

    public void Enter()
    {
        ResetStuckTimer();
        navigationAgent.TargetPosition = currentGoal;
    }

    public void Exit()
    {
        stuckTimer = 0.0f;
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

        if (currentPosition.DistanceTo(currentGoal) <= chaseGoalStopThreshold)
        {
            TransitionEvent?.Invoke(this, nameof(SearchState));
            return;
        }

        if (IsStuck(currentPosition, (float)_delta))
        {
            TransitionEvent?.Invoke(this, nameof(SearchState));
            return;
        }

        Vector3 destination = navigationAgent.GetNextPathPosition();
        Vector3 localDestination = destination - Character.GlobalPosition;
        Vector3 direction = localDestination.Normalized();

        Character.Velocity = direction * CharacterSpeed;
        animationController.PlayMovementAnimation();
        StateRotation.TurnTowards(Character, destination, turnSpeedDegreesPerSecond, (float)_delta);

        Character.MoveAndSlide();
    }

    void ResetStuckTimer()
    {
        stuckTimer = 0.0f;
        previousPosition = Character.GlobalPosition;
        previousPosition.Y = 0.0f;
    }

    bool IsStuck(Vector3 currentPosition, float delta)
    {
        if (currentPosition.DistanceSquaredTo(previousPosition) <= stuckDistanceThreshold * stuckDistanceThreshold)
            stuckTimer += delta;
        else
        {
            stuckTimer = 0.0f;
            previousPosition = currentPosition;
        }

        return stuckTimer >= stuckDisengageTime;
    }
}
