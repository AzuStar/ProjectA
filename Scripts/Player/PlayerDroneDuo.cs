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

    public override void _Input(InputEvent @event)
    {
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
        _droneSummoned = false;
        drone.DisableDrone();
        player.acceptInput = true;
        player.DriveCameraSmoothingTarget = true;
    }

    private void ToggleDrone()
    {
        if (_droneSummoned)
        {
            DisableDrone();
            return;
        }

        _droneSummoned = true;
        drone.EnableDrone(player.GlobalPosition);
        SetActiveCharacter(droneActive: true);
    }

    private void SetActiveCharacter(bool droneActive)
    {
        player.acceptInput = !droneActive;
        player.DriveCameraSmoothingTarget = !droneActive;

        drone.acceptInput = droneActive;
        drone.DriveCameraSmoothingTarget = droneActive;
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
