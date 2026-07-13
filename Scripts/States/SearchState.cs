using System;
using Godot;

public partial class SearchState : Node, IState
{
    public event EventHandler<IState, String> TransitionEvent;

    [Export]
    Timer searchTimer;

    CharacterBody3D Character;
    float SearchTime;
    float TurnDurations;
    Tween tween;

    public void SetCharacter(CharacterBody3D value) => Character = value;

    public void SetSearchTime(float value) => SearchTime = value;

    public void SetTurnDurations(float value) => TurnDurations = value;

    public void Enter()
    {
        GD.Print("Searching Enter");
        searchTimer.Timeout += OnTimerTimeOut;
        searchTimer.WaitTime = SearchTime;
        searchTimer.Start();

        tween = GetTree().CreateTween();
        tween.SetLoops();

        Vector3 left = Vector3.Zero;
        left.Y = Mathf.DegToRad(Character.RotationDegrees.Y - 90f);
        Vector3 right = Vector3.Zero;
        right.Y = Mathf.DegToRad(Character.RotationDegrees.Y + 90f);

        tween.TweenProperty(Character, "rotation", left, TurnDurations);
        tween.TweenProperty(Character, "rotation", right, TurnDurations);
        return;
    }

    public void Exit()
    {
        searchTimer.Timeout -= OnTimerTimeOut;
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
        TransitionEvent?.Invoke(this, "PatrollingState");
    }

    public void TargetFound()
    {
        TransitionEvent?.Invoke(this, "ChasingState");
    }
}
