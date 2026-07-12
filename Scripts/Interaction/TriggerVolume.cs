using System.Diagnostics;
using Godot;
using ProjectA.Game.Player;

namespace ProjectA.Game.Interaction;

public partial class TriggerVolume : Area3D
{
    [Export] private Triggerable _triggerable;

    public override void _Ready()
    {
        Debug.Assert(_triggerable != null, $"Trigger volume {Name} has no target triggerable set!");

        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }

    private void OnBodyEntered(Node3D body)
    {
        if (body is not PlayerCharacterController) // Toggle export? Collision layers?
        {
            return;
        }

        _triggerable.Activate();
    }

    private void OnBodyExited(Node3D body)
    {
        if (body is not PlayerCharacterController) // Toggle export? Collision layers?
        {
            return;
        }

        _triggerable.Deactivate();
    }
}