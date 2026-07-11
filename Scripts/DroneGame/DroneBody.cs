using Godot;

public partial class DroneBody : CharacterBody3D
{
	[Export] private float _maxSpeed = 10.0f;
	[Export] private float _acceleration = 10.0f;
	
	private bool _inputActive;
	
	public override void _Ready()
	{
		_inputActive = false;
	}

	public override void _Process(double delta)
	{
		Vector3 direction = GetMovementInput();
		Vector3 desiredVelocity;
		if (direction.LengthSquared() > 0.0f)
		{
			desiredVelocity = direction * _maxSpeed;
		}
		else
		{
			desiredVelocity = Vector3.Zero;
		}

		Velocity = Velocity.MoveToward(desiredVelocity, _acceleration * (float)delta);

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
