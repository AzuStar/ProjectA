using System.Diagnostics;
using Godot;
using ProjectA.Game.Player;

namespace ProjectA.Game.Interaction;

public partial class TriggerVolume : Area3D
{
    /// <summary>
    /// The triggerable this trigger volume will activate and deactivate.
    /// </summary>
    [Export]
    private Triggerable _triggerable;

    /// <summary>
    /// If set, activate once on entry and then never again, and never deactivate on exit.
    /// </summary>
    [Export]
    private bool _oneShot;

    public override void _Ready()
    {
        Debug.Assert(_triggerable != null, $"Trigger volume {Name} has no target triggerable set!");

        BodyEntered += OnBodyEntered;
        if (!_oneShot)
        {
            BodyExited += OnBodyExited;
        }
    }

    private void OnBodyEntered(Node3D body)
    {
        if (body is not PlayerCharacterController) // Toggle export? Collision layers?
        {
            return;
        }

        _triggerable.Activate();

        if (_oneShot)
        {
            // We're done, goodbye!
            BodyEntered -= OnBodyEntered;
        }
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
