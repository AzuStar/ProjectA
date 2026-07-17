using Godot;

public partial class PushableBody : CharacterBody3D
{
    private Vector3 _initialPosition;
    private bool _pendingRespawn;

    public override void _Ready()
    {
        base._Ready();
        _initialPosition = GlobalPosition;
        _pendingRespawn = false;
    }

    public bool TryPush(Vector2 pushXz)
    {
        Vector3 velocity = Velocity;
        velocity.X = pushXz.X;
        velocity.Z = pushXz.Y;
        Velocity = velocity;

        MoveAndSlide();

        return Velocity.LengthSquared() > 0.0f;
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
            return;
        }
        
		Vector3 velocity = Velocity;
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

        velocity.X = 0.0f;
        velocity.Z = 0.0f;
		Velocity = velocity;

		MoveAndSlide();
	}
}
