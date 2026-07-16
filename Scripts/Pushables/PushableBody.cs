using Godot;

public partial class PushableBody : CharacterBody3D
{
    public bool TryPush(Vector2 pushXz)
    {
        Vector3 velocity = Velocity;
        velocity.X = pushXz.X;
        velocity.Z = pushXz.Y;
        Velocity = velocity;

        MoveAndSlide();

        return Velocity.LengthSquared() > 0.0f;
    }

    public override void _PhysicsProcess(double delta)
	{
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
