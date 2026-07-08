using Godot;

namespace ProjectA.Game;

[Tool]
public partial class ParentAxisRotator3D : Node
{
    [Export]
    public Vector3 rotationSpeedDegreesPerSecond = new(0.0f, 180.0f, 0.0f);

    [Export]
    public bool enabledInEditor = false;

    private Node3D? _target;

    public override void _Ready()
    {
        RefreshTarget();
    }

    public override void _Notification(int what)
    {
        if (what == NotificationParented)
            RefreshTarget();
        else if (what == NotificationUnparented)
            _target = null;
    }

    public override void _Process(double delta)
    {
        if (Engine.IsEditorHint() && !enabledInEditor)
            return;

        if (_target == null)
            return;

        _target.RotationDegrees += rotationSpeedDegreesPerSecond * (float)delta;
    }

    private void RefreshTarget()
    {
        _target = GetParent<Node3D>();

        if (_target == null)
            GD.PushError($"{nameof(ParentAxisRotator3D)} requires direct parent to be Node3D.");
    }
}
