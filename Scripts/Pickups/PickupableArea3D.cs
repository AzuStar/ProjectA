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
    private bool _pickedUp;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node3D body)
    {
        if (_pickedUp || body is not PlayerCharacterController)
            return;

        _pickedUp = true;
        PropogateEvent_IPickupReaction(body);
        GameManagerSingleton.currentLevelInstance.AddItem(itemType);
        rootNode.QueueFree();
    }

    private void PropogateEvent_IPickupReaction(Node3D body)
    {
        foreach (Node child in GetChildren())
            if (child is IPickupReaction pickupReactionEvent)
                pickupReactionEvent.Emit(this, body);
    }
}
