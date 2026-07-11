using Godot;

public partial class PlayerController : CharacterBody3D
{
	private bool _inputActive;

	private const float Speed = 5.0f;
	private const float JumpVelocity = 4.5f;

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (_inputActive && Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		Vector3 direction = GetMovementInput();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	private Vector3 GetMovementInput()
	{
		if (_inputActive)
		{
			Vector2 inputDir = Input.GetVector("game_left", "game_right", "game_forward", "game_back");
			Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
			return direction;
		}
		else
		{
			return Vector3.Zero;
		}
	}

	public void SetInputActive(bool inputActive)
	{
		_inputActive = inputActive;
	}
}
