using Godot;
using ProjectA.Game.Interaction;
using ProjectA.Game.Inventory;
using ProjectA.Game.Levels;
using ProjectA.Game.Player;

namespace ProjectA.Game.Barriers;

public partial class BarrierDoor : Barrier
{
    [Export]
    public Area3D unlockArea;

    [Export]
    public NavigationLink3D navigationLink;

    public override void _Ready()
    {
        base._Ready();
        unlockArea.BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node3D body)
    {
        if (body is not PlayerCharacterController)
            return;

        // This region is when you're approaching a door with a key to unlock it.
        // Doesn't apply when pressing a trigger somewhere else.
        if (!LevelInstance.Current.RemoveOneItem(ItemType.Key))
            return;

        TryOpen();
    }

    public override bool TryOpen()
    {
        bool baseSuccess = base.TryOpen();

        if (baseSuccess)
        {
            // Let non-players pass through the door
            navigationLink.Enabled = true;
        }

        return baseSuccess;
    }

    public override bool TryClose()
    {
        bool baseSuccess = base.TryClose();

        if (baseSuccess)
        {
            // Non-players can't walk this anymore
            navigationLink.Enabled = false;
        }

        return baseSuccess;
    }
}
