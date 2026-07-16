using System;
using Godot;
using ProjectA.Game.Enemies;

public partial class PatrollingState : Node, IState
{
    public event EventHandler<IState, String> TransitionEvent;

    [Export]
    NavigationAgent3D navigationAgent;

    Path3D patrolPath;

    [Export]
    public float CharacterSpeed;

    [Export]
    public float turnSpeedDegreesPerSecond = 360.0f;

    [Export]
    public float patrolGoalStopThreshold = 0.5f;

    int currentIndex = 0;
    CharacterBody3D Character;
    IEnemyAnimationController animationController;
    Vector3 currentGoal;

    public void SetPatrolPath(Path3D value) => patrolPath = value;

    public void SetCharacterSpeed(float _speed) => CharacterSpeed = _speed;

    public void SetCharacter(CharacterBody3D _character) => Character = _character;

    public void SetAnimationController(IEnemyAnimationController value) => animationController = value;

    public void Enter()
    {
        //TransitionEvent?.Invoke(this,"ChasingState");
        SetCurrentGoal();
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
        Vector3 currentPosition = Flatten(Character.GlobalPosition);
        Vector3 reachableGoal = Flatten(navigationAgent.GetFinalPosition());

        if (
            navigationAgent.IsNavigationFinished()
            || currentPosition.DistanceTo(reachableGoal) <= patrolGoalStopThreshold
        )
        {
            currentIndex++;

            if (currentIndex >= patrolPath.Curve.PointCount)
            {
                currentIndex = 0;
            }

            SetCurrentGoal();

            navigationAgent.TargetPosition = currentGoal;

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

        return;
    }

    void SetCurrentGoal()
    {
        currentGoal = patrolPath.GlobalTransform * patrolPath.Curve.GetPointPosition(currentIndex);
        currentGoal = NavigationServer3D.MapGetClosestPoint(navigationAgent.GetNavigationMap(), currentGoal);
    }

    static Vector3 Flatten(Vector3 position)
    {
        position.Y = 0.0f;
        return position;
    }

    public void ChaseTime() => TransitionEvent?.Invoke(this, nameof(ChasingState));
}
