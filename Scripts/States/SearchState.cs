using System;
using Godot;
using ProjectA.Game.Enemies;

public partial class SearchState : Node, IState
{
    public event EventHandler<IState, String> TransitionEvent;

    [Export]
    Timer searchTimer;

    [Export]
    Node3D nodeToManipulate;

    [Export]
    float turnSpeedDegreesPerSecond = 180.0f;

    IEnemyAnimationController animationController;
    Tween tween;

    public void SetAnimationController(IEnemyAnimationController value) => animationController = value;

    public override void _EnterTree()
    {
        base._EnterTree();
        searchTimer.Timeout += OnTimerTimeOut;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        searchTimer.Timeout -= OnTimerTimeOut;
    }

    public void Enter()
    {
        animationController.PlayIdleAnimation();
        searchTimer.Start();

        tween = GetTree().CreateTween();
        tween.SetLoops();

        Vector3 left = nodeToManipulate.Rotation;
        left.Y -= Mathf.DegToRad(90.0f);
        Vector3 right = nodeToManipulate.Rotation;
        right.Y += Mathf.DegToRad(90.0f);

        tween.TweenProperty(nodeToManipulate, "rotation", left, GetTurnDuration(90.0f));
        tween.TweenProperty(nodeToManipulate, "rotation", right, GetTurnDuration(180.0f));
        tween.TweenProperty(nodeToManipulate, "rotation", left, GetTurnDuration(180.0f));
        return;
    }

    public void Exit()
    {
        searchTimer.Stop();

        tween?.Kill();
        tween = null;
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

    void OnTimerTimeOut()
    {
        TransitionEvent?.Invoke(this, nameof(PatrollingState));
    }

    public void TargetFound()
    {
        TransitionEvent?.Invoke(this, nameof(ChasingState));
    }

    float GetTurnDuration(float angleDegrees) => angleDegrees / turnSpeedDegreesPerSecond;
}
