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

    public DuoTarget currentlyActivePart = DuoTarget.PLAYER;

    // Have to delay this because the player is not respawned when the level restarts so the physics system still sees trigger overlaps
    // This waits for half a second, a cleaner fix probably exists, if you have time go ahead
    public bool IsPrepared => _isPrepared && Time.GetTicksMsec() > _preparationTime + 500UL;

    public override void _Ready()
    {
        _isPrepared = false;
        _preparationTime = 0UL;
        currentlyActivePart = DuoTarget.PLAYER;
    }

    public void HandleInput(InputEvent @event, float mouseSensitivity)
    {
        if (!ActivePartAcceptsInput())
            return;

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

    private void DisableDrone()
    {
        //Input.MouseMode = Input.MouseModeEnum.Captured;
        currentlyActivePart = DuoTarget.PLAYER;
        drone.LeaveThisController();
        player.EnterThisController();
    }

    private void EnableDrone()
    {
        //Input.MouseMode = Input.MouseModeEnum.Captured;
        currentlyActivePart = DuoTarget.DRONE;
        player.LeaveThisController();
        drone.EnterThisController(player.GlobalPosition, player.GlobalRotation.Y, player);
    }

    private void ToggleDrone()
    {
        if (currentlyActivePart == DuoTarget.DRONE)
        {
            DisableDrone();
            return;
        }

        EnableDrone();
    }

    private FpsCamera GetActiveCamera()
    {
        return currentlyActivePart == DuoTarget.DRONE ? drone.fpsCamera : player.fpsCamera;
    }

    private bool ActivePartAcceptsInput()
    {
        return currentlyActivePart == DuoTarget.DRONE ? drone.acceptInput : player.acceptInput;
    }

    public void Prepare(Vector3 spawnPosition)
    {
        player.GlobalPosition = spawnPosition;
        player.ProcessMode = ProcessModeEnum.Inherit;
        player.acceptInput = true;

        DisableDrone();

        GetActiveCamera().ResetOrientation();

        _isPrepared = true;
        _preparationTime = Time.GetTicksMsec();
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
