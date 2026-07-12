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
    private ulong _preparationTime;
    private bool _droneSummoned;

    // Have to delay this because the player is not respawned when the level restarts so the physics system still sees trigger overlaps
    // This waits for half a second, a cleaner fix probably exists, if you have time go ahead
    public bool IsPrepared => _isPrepared && Time.GetTicksMsec() > _preparationTime + 500UL;

    public override void _Ready()
    {
        _isPrepared = false;
        _preparationTime = 0UL;
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

    private void SetDroneEnabled(bool droneEnabled)
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        _droneSummoned = droneEnabled;
        if (!droneEnabled)
        {
            drone.DisableDrone();
            player.EnablePlayer();
        }
        else
        {
            player.DisablePlayerForDrone();
            drone.EnableDrone(player.GlobalPosition);
        }
    }

    private void ToggleDrone()
    {
        SetDroneEnabled(!_droneSummoned);
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

        SetDroneEnabled(false);

        GetActiveCamera().ResetOrientation();

        _isPrepared = true;
        _preparationTime = Time.GetTicksMsec();
    }

    public void Unprepare()
    {
        SetDroneEnabled(false);

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
