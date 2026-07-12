using Godot;
using ProjectA.Game.Player;
using ProjectA.Game.Singletons;

namespace ProjectA.Game;

public partial class PickupArea3D : Area3D
{
    [Export] private PickupKind _pickupKind;

    private bool _pickedUp;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node3D body)
    {
        if (_pickedUp || body is not PlayerCharacterController)
            return;

        PlayerSingleton? playerSingleton = PlayerSingleton.Instance;
        if (playerSingleton == null)
        {
            GD.PushError($"{nameof(PickupArea3D)} requires {nameof(PlayerSingleton)} autoload.");
            return;
        }
        _pickedUp = true;
        // TODO: Ideally emit signals instead?
        switch (_pickupKind)
        {
            case PickupKind.Coin:
                playerSingleton.CoinsCollected++;
                break;
            case PickupKind.Key:
                playerSingleton.KeysHeld++;
                break;
        }
        QueueFree();
    }
}
