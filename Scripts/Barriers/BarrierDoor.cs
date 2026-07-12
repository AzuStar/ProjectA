using Godot;
using ProjectA.Game.Inventory;
using ProjectA.Game.Levels;
using ProjectA.Game.Player;

namespace ProjectA.Game.Barriers;

public partial class BarrierDoor : Barrier
{
    [Export]
    public Area3D unlockArea;

    [Export]
    public NavigationObstacle3D navigationObstacle;

    public override void _Ready()
    {
        unlockArea.BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node3D body)
    {
        if (isOpen || body is not PlayerCharacterController)
            return;

        if (!LevelInstance.Current.RemoveOneItem(ItemType.Key))
            return;

        Open();

        navigationObstacle.AffectNavigationMesh = false;
    }
}
