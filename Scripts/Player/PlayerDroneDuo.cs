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

    public DuoTarget currentlyActivePart = DuoTarget.PLAYER;

    public override void _Ready()
    {
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

        GetActiveCamera().ResetPose();
    }

    public void Unprepare()
    {
        DisableDrone();
    }

    public void Kill()
    {
        // Both parts become unusable.
        player.acceptInput = false;
        drone.acceptInput = false;
    }
}
