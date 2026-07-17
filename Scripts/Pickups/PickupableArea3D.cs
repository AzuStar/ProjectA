using Godot;
using ProjectA.Game.Inventory;
using ProjectA.Game.Player;
using ProjectA.Game.Singletons;

namespace ProjectA.Game.Pickups;

public partial class PickupableArea3D : Area3D
{
    [Export]
    public Node3D rootNode;

    [Export]
    public ItemType itemType;
    protected bool pickedUp;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    protected virtual void OnBodyEntered(Node3D body)
    {
        if (pickedUp || body is not PlayerCharacterController)
            return;

        PickUp(body);
        GameManagerSingleton.currentLevelInstance.AddItem(itemType, 1);
        rootNode.QueueFree();
    }

    protected void PickUp(Node3D body)
    {
        pickedUp = true;

        PropogateEvent_IPickupReaction(body);
    }

    private void PropogateEvent_IPickupReaction(Node3D body)
    {
        foreach (Node child in GetChildren())
            if (child is IPickupReaction pickupReactionEvent)
                pickupReactionEvent.Emit(this, body);
    }
}
