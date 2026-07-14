using Godot;
using ProjectA.Game.Tables;

namespace ProjectA.Game.Player;

public partial class PlayerDroneDuo : Node3D
{
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
            GetActiveCameraRig().ApplyMouseLook(mouseMotion.Relative, mouseSensitivity);
        }

        if (@event is not InputEventKey { Pressed: true } keyEvent || keyEvent.IsEcho())
            return;

        if (@event.IsActionPressed(InputsTable.ACTION_TOGGLE_DRONE))
        {
            ToggleDrone();
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

    private ThirdPersonCameraRig GetActiveCameraRig()
    {
        return currentlyActivePart == DuoTarget.DRONE ? drone.cameraRig : player.cameraRig;
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

        //GetActiveCamera().ResetPose();
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
