using Godot;
using ProjectA.Game.Interaction;
using ProjectA.Game.Inventory;
using ProjectA.Game.Levels;
using ProjectA.Game.Player;
using ProjectA.Game.Singletons;

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

        // Do we have a key to open the door with?
        // This region is when you're approaching a door with a key to unlock it.
        // Doesn't apply when pressing a trigger somewhere else.
        // Subtracting the key after opening as it might still fail for other reasons.
        if (!IsOpen && !GameManagerSingleton.currentLevelInstance.HasItem(ItemType.Key))
            return;

        bool opened = TryOpen();
        if (opened)
        {
            GameManagerSingleton.currentLevelInstance.AddItem(ItemType.Key, -1);
        }
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
