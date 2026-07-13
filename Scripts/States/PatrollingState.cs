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

        if (currentPosition.DistanceTo(currentGoal) <= 0.5)
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
        Vector3 localDestination = destination - Character.GlobalPosition;
        Vector3 direction = localDestination.Normalized();

        Character.Velocity = direction * CharacterSpeed;
        animationController.PlayMovementAnimation();

        if (localDestination.LengthSquared() > 0.0001f)
            Character.LookAt(destination);

        Character.MoveAndSlide();

        return;
    }

    void SetCurrentGoal()
    {
        currentGoal = patrolPath.GlobalTransform * patrolPath.Curve.GetPointPosition(currentIndex);
        currentGoal.Y = 0.0f;
    }

    public void ChaseTime() => TransitionEvent?.Invoke(this, nameof(ChasingState));
}
