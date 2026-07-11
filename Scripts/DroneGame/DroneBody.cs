using Godot;

public partial class DroneBody : CharacterBody3D
{
	[Export] private float _maxSpeed = 10.0f;
	[Export] private float _acceleration = 10.0f;
	[Export] private float _lookSensitivity = 1.0f;
	
	private bool _inputActive;
	private Vector2 _pendingLookMotion;

	public override void _Ready()
	{
		_inputActive = false;
	}

	public void PassInput(InputEvent ev)
	{
		if (ev is InputEventMouseMotion mouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			_pendingLookMotion = mouseMotion.Relative;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		ProcessLook(delta);
		ProcessMovement(delta);
	}

	private void ProcessLook(double delta)
	{
		if (!_inputActive)
		{
			return;
		}

		if (_pendingLookMotion.LengthSquared() > 0.0f)
		{
			float sens = _lookSensitivity * 0.01f;
			Rotate(Vector3.Down, _pendingLookMotion.X * sens * (float)delta);
			_pendingLookMotion = Vector2.Zero;	
		}
	}

	private void ProcessMovement(double delta)
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
