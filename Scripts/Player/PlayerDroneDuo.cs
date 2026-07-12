using Godot;
using ProjectA.Game.Singletons;
using ProjectA.Game.Tables;

namespace ProjectA.Game.Player;

public partial class PlayerDroneDuo : Node3D
{
    [Export]
    public PlayerCharacterController player;

    [Export]
    public DroneCharacterController drone;

    public DuoTarget currentlyActivePart = DuoTarget.PLAYER;

    public override void _ExitTree()
    {
        base._ExitTree();
    }

    public void InputProcess(InputEvent input, float mouseSensitivity)
    {
        if (!ActivePartAcceptsInput())
            return;

        if (@input is InputEventMouseMotion mouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            GetActiveCamera().ApplyMouseLook(mouseMotion.Relative, mouseSensitivity);
            return;
        }

        if (@input.IsActionPressed(InputsTable.ACTION_TOGGLE_DRONE))
        {
            ToggleDrone();
            return;
        }
    }

    private FpsCamera GetActiveCamera()
    {
        return currentlyActivePart == DuoTarget.DRONE ? drone.fpsCamera : player.fpsCamera;
    }

    private bool ActivePartAcceptsInput()
    {
        return currentlyActivePart == DuoTarget.DRONE ? drone.acceptInput : player.acceptInput;
    }

    private void DisableDrone()
    {
        currentlyActivePart = DuoTarget.PLAYER;
        drone.LeaveThisController();
        player.EnterThisController();
    }

    private void EnableDrone()
    {
        currentlyActivePart = DuoTarget.DRONE;
        player.LeaveThisController();
        drone.EnterThisController(player.GlobalPosition);
    }

    public void ToggleDrone()
    {
        if (currentlyActivePart == DuoTarget.DRONE)
        {
            DisableDrone();
            return;
        }

        EnableDrone();
    }

    public void Prepare(Vector3 spawnPosition)
    {
        player.GlobalPosition = spawnPosition;
        player.acceptInput = true;

        DisableDrone();

        GetActiveCamera().ResetOrientation();
    }

    public void Kill()
    {
        currentlyActivePart = DuoTarget.PLAYER;
        drone.LeaveThisController();
        player.Die();
    }
}
