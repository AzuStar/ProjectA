using Godot;
using ProjectA.Game.Levels;
using ProjectA.Game.Player;
using ProjectA.Game.Pickups;
using ProjectA.Game.Singletons;

public partial class KillTrigger : Area3D
{
    [ExportGroup("Respawning")]
    [Export] private bool _respawnsPushables;
    [Export] private RespawnTrigger _respawnTrigger;

    public override void _Ready()
    {
        BodyEntered += HandleBodyEntered;
        AreaEntered += HandleAreaEntered;

        if (_respawnTrigger != null)
        {
            _respawnTrigger.SetEnabled(_respawnsPushables);
        }
    }

    private void HandleBodyEntered(Node3D body)
    {
        if (body is PlayerCharacterController characterController)
        {
            GameManagerSingleton.currentLevelInstance.CurrentGameState = LevelInstance.GameState.Dead;
            return;
        }
        if (body is DroneCharacterController droneController)
        {
            droneController.DropCarriedPickup();
            PlayerSingleton.Instance.playerDuo.DisableDrone();
            droneController.StartDeathTriggerCooldown();
            return;
        }
    }

    private void HandleAreaEntered(Area3D area)
    {
        if (area is PickupableByDroneArea3D pickupable)
            pickupable.Respawn();
    }
}
