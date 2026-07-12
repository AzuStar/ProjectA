using Godot;
using ProjectA.Game.Player;
using ProjectA.Game.Levels;
using ProjectA.Game.Inventory;
using ProjectA.Game.Singletons;

namespace ProjectA.Game;

public partial class KeyPickupArea3D : Area3D
{
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
        GameManagerSingleton.currentLevelInstance.AddItem(new KeyItem());
        QueueFree();
    }
}
