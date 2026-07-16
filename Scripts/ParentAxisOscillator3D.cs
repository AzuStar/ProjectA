using Godot;

namespace ProjectA.Game;

[Tool]
public partial class ParentAxisOscillator3D : Node
{
    [Export]
    public Vector3 rotationSpeedDegreesPerSecond = new(00.0f, 0.0f, 0.0f);

    [Export]
    public Vector3 minRotationDegrees = new(00.0f, 0.0f, 0.0f);

    [Export]
    public Vector3 maxRotationDegrees = new(0.0f, 0.0f, 0.0f);

    [Export]
    public bool enabledInEditor = false;

    private Node3D? _target;
    private int _xDirection = 1;
    private int _yDirection = 1;
    private int _zDirection = 1;

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

        Vector3 rotation = _target.RotationDegrees;
        float deltaSeconds = (float)delta;

        rotation.X = MoveAxis(
            rotation.X,
            minRotationDegrees.X,
            maxRotationDegrees.X,
            rotationSpeedDegreesPerSecond.X,
            ref _xDirection,
            deltaSeconds
        );
        rotation.Y = MoveAxis(
            rotation.Y,
            minRotationDegrees.Y,
            maxRotationDegrees.Y,
            rotationSpeedDegreesPerSecond.Y,
            ref _yDirection,
            deltaSeconds
        );
        rotation.Z = MoveAxis(
            rotation.Z,
            minRotationDegrees.Z,
            maxRotationDegrees.Z,
            rotationSpeedDegreesPerSecond.Z,
            ref _zDirection,
            deltaSeconds
        );

        _target.RotationDegrees = rotation;
    }

    private float MoveAxis(float value, float min, float max, float speed, ref int direction, float delta)
    {
        float lower = Mathf.Min(min, max);
        float upper = Mathf.Max(min, max);
        float axisSpeed = Mathf.Abs(speed);

        if (axisSpeed == 0.0f || lower == upper)
            return Mathf.Clamp(value, lower, upper);

        float target = direction > 0 ? upper : lower;
        value = Mathf.MoveToward(value, target, axisSpeed * delta);

        if (value == target)
            direction *= -1;

        return value;
    }

    private void RefreshTarget()
    {
        _target = GetParent<Node3D>();

        if (_target == null)
            GD.PushError($"{nameof(ParentAxisOscillator3D)} requires direct parent to be Node3D.");
    }
}
