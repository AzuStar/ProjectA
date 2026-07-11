using Godot;
using ProjectA.Game.Player;

namespace ProjectA.Game;

public partial class TorchLighterArea3D : Area3D
{
    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node3D body)
    {
        if (body is PlayerCharacterController or DroneCharacterController)
        {
            GetParent<Torch>()?.Lighten();
        }
    }
}
