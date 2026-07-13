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
    float TurnDurations;
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

        Vector3 left = Vector3.Zero;
        left.Y = Mathf.DegToRad(nodeToManipulate.RotationDegrees.Y - 90f);
        Vector3 right = Vector3.Zero;
        right.Y = Mathf.DegToRad(nodeToManipulate.RotationDegrees.Y + 90f);

        tween.TweenProperty(nodeToManipulate, "rotation", left, TurnDurations);
        tween.TweenProperty(nodeToManipulate, "rotation", right, TurnDurations);
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
}
