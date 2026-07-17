using Godot;

public partial class PushableBodyRespawnable : PushableBody
{
    private bool _pendingRespawn;
    private Vector3 _initialPosition;

    public override void _Ready()
    {
        base._Ready();
        _pendingRespawn = false;
        _initialPosition = GlobalPosition;
    }

    public void Respawn()
    {
        PhysicsInterpolationMode = PhysicsInterpolationModeEnum.Off;
        _pendingRespawn = true;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_pendingRespawn)
        {
            Velocity = Vector3.Zero;
            GlobalPosition = _initialPosition;
            _pendingRespawn = false;
            PhysicsInterpolationMode = PhysicsInterpolationModeEnum.Inherit;
        }
        else
        {
            base._PhysicsProcess(delta);
        }
    }
}