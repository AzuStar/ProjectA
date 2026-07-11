using Godot;

public partial class DroneBody : CharacterBody3D
{
	private bool _inputActive;
	
	public override void _Ready()
	{
		_inputActive = false;
	}

	public override void _Process(double delta)
	{
		Vector3 direction = GetMovementInput();
		if (direction.LengthSquared() > 0.0f)
		{
			Velocity = direction;
		}
		else
		{
			Velocity = Vector3.Zero;
		}

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
