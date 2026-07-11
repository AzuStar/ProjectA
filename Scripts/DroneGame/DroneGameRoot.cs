using Godot;

public partial class DroneGameRoot : Node
{
	[Export] private PlayerController _player;
	[Export] private DroneHandler _droneHandler;

	private bool _isDroneActive;

	public override void _Ready()
	{
		SetDroneActiveState(false, true);
	}

	public override void _Input(InputEvent ev)
	{
		if (ev.IsActionPressed("toggle_drone"))
		{
			SetDroneActiveState(!_isDroneActive);
		}
	}

	private void SetDroneActiveState(bool active, bool force = false)
	{
		if (_isDroneActive == active && !force)
		{
			return;
		}

		_isDroneActive = active;

		_player.SetInputActive(!_isDroneActive);
		_droneHandler.SetDroneInputActive(_isDroneActive);
	}
}
