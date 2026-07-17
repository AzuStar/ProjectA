using Godot;
using ProjectA.Game.Player;
using ProjectA.Game.Singletons;

namespace ProjectA.Game.Pickups;

[GlobalClass]
public partial class PickupableByDroneArea3D : PickupableArea3D
{
    private Node _spawnParent;
    private Transform3D _spawnTransform;
    private DroneCharacterController _carryingDrone;

    public override void _Ready()
    {
        _spawnParent = rootNode.GetParent();
        _spawnTransform = rootNode.GlobalTransform;
        base._Ready();
    }

    protected override void OnBodyEntered(Node3D body)
    {
        if (pickedUp)
        {
            if (_carryingDrone != null && body is PlayerCharacterController)
                DeliverToPlayer(body);

            return;
        }

        if (body is DroneCharacterController droneController)
        {
            if (!droneController.TryCarryPickup(this))
                return;

            PickUp(body);
            return;
        }

        base.OnBodyEntered(body);
    }

    private void DeliverToPlayer(Node3D body)
    {
        _carryingDrone.ReleaseCarriedPickup(this);
        _carryingDrone = null;
        PickUp(body);
        GameManagerSingleton.currentLevelInstance.AddItem(itemType, 1);
        rootNode.QueueFree();
    }

    public void AttachToDrone(DroneCharacterController droneController, Node3D droneCarryRoot)
    {
        _carryingDrone = droneController;
        rootNode.Reparent(droneCarryRoot);
        rootNode.Position = Vector3.Zero;
        rootNode.Rotation = Vector3.Zero;
    }

    public void Drop(Vector3 globalPosition)
    {
        _carryingDrone = null;
        rootNode.Reparent(_spawnParent);
        rootNode.GlobalPosition = globalPosition;
        pickedUp = false;
    }

    public void Respawn()
    {
        if (_carryingDrone != null)
            _carryingDrone.ReleaseCarriedPickup(this);

        _carryingDrone = null;
        rootNode.Reparent(_spawnParent);
        rootNode.GlobalTransform = _spawnTransform;
        pickedUp = false;
    }
}
