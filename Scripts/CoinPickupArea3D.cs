using Godot;

namespace ProjectA.Game;

public partial class CoinPickupArea3D : Area3D
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

        PlayerSingleton? playerSingleton = PlayerSingleton.Instance;
        if (playerSingleton == null)
        {
            GD.PushError($"{nameof(CoinPickupArea3D)} requires {nameof(PlayerSingleton)} autoload.");
            return;
        }

        _pickedUp = true;
        playerSingleton.CoinsCollected++;
        QueueFree();
    }
}
