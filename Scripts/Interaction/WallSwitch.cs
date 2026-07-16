using Godot;

namespace ProjectA.Game.Interaction;

public partial class WallSwitch : LeverSwitch
{
    [Export]
    public Node3D switchToSquish;

    [Export]
    public Vector3 squishTo;

    [Export]
    public float squishOverTime = 0.25f;

    private Vector3 _unsquishedScale;

    public override void _Ready()
    {
        _unsquishedScale = switchToSquish.Scale;
    }

    protected override void Activated()
    {
        base.Activated();
        CreateTween().TweenProperty(switchToSquish, "scale", squishTo, squishOverTime);
    }

    protected override void Deactivated()
    {
        base.Deactivated();
        CreateTween().TweenProperty(switchToSquish, "scale", _unsquishedScale, squishOverTime);
    }
}
