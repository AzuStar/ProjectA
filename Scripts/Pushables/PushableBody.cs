using Godot;
using System;

public partial class PushableBody : CharacterBody3D
{
    private Vector2 _pendingPush;

    public void NotifyPush(Vector2 pushXz)
    {
        _pendingPush += pushXz;
    }

    public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

        velocity.X = _pendingPush.X;
        velocity.Z = _pendingPush.Y;
		Velocity = velocity;
        _pendingPush = Vector2.Zero;

		MoveAndSlide();
	}
}
