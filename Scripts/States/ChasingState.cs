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
    public float chaseGoalStopThreshold = 0.75f;

    [Export]
    public float turnSpeedDegreesPerSecond = 360.0f;

    [Export]
    public float stuckDisengageTime = 1.5f;

    [Export]
    public float stuckDistanceThreshold = 0.02f;

    [Export]
    public float stuckProgressThreshold = 0.05f;

    float stuckTimer;
    Vector3 previousPosition;
    float bestGoalDistance;

    public void SetCharacterSpeed(float value) => CharacterSpeed = value;

    public void SetCharacter(CharacterBody3D value) => Character = value;

    public void SetAnimationController(IEnemyAnimationController value) => animationController = value;

    public uint NavigationMapIteration()
    {
        return NavigationServer3D.MapGetIterationId(navigationAgent.GetNavigationMap());
    }

    public bool NavigationMapReady()
    {
        return NavigationMapIteration() != 0;
    }

    public bool NavigationMapReadyAfter(uint iteration)
    {
        uint currentIteration = NavigationMapIteration();
        return currentIteration != 0 && currentIteration != iteration;
    }

    public void SetIdleTarget(Vector3 position)
    {
        navigationAgent.TargetPosition = Flatten(position);
    }

    public void SetGoal(Vector3 goalPosition)
    {
        currentGoal = GetClosestNavigationPoint(goalPosition);
        navigationAgent.TargetPosition = currentGoal;
    }

    public void Enter()
    {
        navigationAgent.TargetPosition = currentGoal;
        ResetStuckTimer();
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
        Vector3 currentPosition = Flatten(Character.GlobalPosition);
        Vector3 reachableGoal = Flatten(navigationAgent.GetFinalPosition());

        if (navigationAgent.IsNavigationFinished() || currentPosition.DistanceTo(reachableGoal) <= chaseGoalStopThreshold)
        {
            GoToSearch();
            return;
        }

        if (IsStuck(currentPosition, reachableGoal, (float)_delta))
        {
            GoToSearch();
            return;
        }

        Vector3 destination = navigationAgent.GetNextPathPosition();
        destination.Y = Character.GlobalPosition.Y;
        Vector3 localDestination = destination - Character.GlobalPosition;
        localDestination.Y = 0.0f;
        Vector3 direction = localDestination.Normalized();

        Character.Velocity = direction * CharacterSpeed;
        animationController.PlayMovementAnimation();
        StateRotation.TurnTowards(Character, destination, turnSpeedDegreesPerSecond, (float)_delta);

        Character.MoveAndSlide();
    }

    void ResetStuckTimer()
    {
        stuckTimer = 0.0f;
        previousPosition = Flatten(Character.GlobalPosition);
        bestGoalDistance = previousPosition.DistanceTo(Flatten(navigationAgent.GetFinalPosition()));
    }

    bool IsStuck(Vector3 currentPosition, Vector3 reachableGoal, float delta)
    {
        float goalDistance = currentPosition.DistanceTo(reachableGoal);
        bool barelyMoved = currentPosition.DistanceSquaredTo(previousPosition) <= stuckDistanceThreshold * stuckDistanceThreshold;
        bool noProgress = goalDistance >= bestGoalDistance - stuckProgressThreshold;

        if (barelyMoved || noProgress)
            stuckTimer += delta;
        else
        {
            stuckTimer = 0.0f;
            previousPosition = currentPosition;
            bestGoalDistance = goalDistance;
        }

        return stuckTimer >= stuckDisengageTime;
    }

    Vector3 GetClosestNavigationPoint(Vector3 position)
    {
        if (!NavigationMapReady())
            return Flatten(position);

        return NavigationServer3D.MapGetClosestPoint(navigationAgent.GetNavigationMap(), position);
    }

    static Vector3 Flatten(Vector3 position)
    {
        position.Y = 0.0f;
        return position;
    }

    void GoToSearch()
    {
        Character.Velocity = Vector3.Zero;
        TransitionEvent?.Invoke(this, nameof(SearchState));
    }
}
