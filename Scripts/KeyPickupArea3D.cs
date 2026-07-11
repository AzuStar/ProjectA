using Godot;
using ProjectA.Game.Player;
using ProjectA.Game.Levels;
using ProjectA.Game.Inventory;

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
        LevelInstance.Current.AddItem(new KeyItem());
        QueueFree();
    }
}
