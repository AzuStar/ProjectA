using Godot;

namespace ProjectA.Game.Player;

public partial class PlayerDroneDuo : Node3D
{
    [Export]
    public Key SummonDroneKey = Key.Q;

    [Export]
    public PlayerCharacterController player;

    [Export]
    public DroneCharacterController drone;

    private bool _isPrepared;
    private bool _droneSummoned;

    public bool IsPrepared => _isPrepared;

    public override void _Ready()
    {
        _isPrepared = false;
        _droneSummoned = false;
    }

    public void HandleInput(InputEvent @event, float mouseSensitivity)
    {
        if (@event is InputEventMouseMotion mouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            GetActiveCamera().ApplyMouseLook(mouseMotion.Relative, mouseSensitivity);
            return;
        }

        if (@event is not InputEventKey { Pressed: true } keyEvent || keyEvent.IsEcho())
            return;

        if (keyEvent.Keycode == SummonDroneKey)
        {
            ToggleDrone();
            return;
        }
    }

    public void DisableDrone()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        _droneSummoned = false;
        drone.DisableDrone();
        player.EnablePlayer();
    }

    private void ToggleDrone()
    {
        if (_droneSummoned)
        {
            DisableDrone();
            return;
        }

        Input.MouseMode = Input.MouseModeEnum.Captured;
        _droneSummoned = true;
        player.DisablePlayerForDrone();
        drone.EnableDrone(player.GlobalPosition);
    }

    private FpsCamera GetActiveCamera()
    {
        return _droneSummoned ? drone.fpsCamera : player.fpsCamera;
    }

    public void Prepare(Vector3 spawnPosition)
    {
        player.GlobalPosition = spawnPosition;
        player.ProcessMode = ProcessModeEnum.Inherit;
        player.acceptInput = true;
        player.DriveCameraSmoothingTarget = true;

        DisableDrone();

        _isPrepared = true;
    }

    public void Unprepare()
    {
        DisableDrone();

        _isPrepared = false;
    }

    public void Kill()
    {
        // Both parts become unusable.
        player.acceptInput = false;
        drone.acceptInput = false;
        _isPrepared = false;
    }
}
