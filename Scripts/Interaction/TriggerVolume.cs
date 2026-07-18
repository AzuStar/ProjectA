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

    [Export]
    public Node3D nodeToSquish;

    [Export]
    public Vector3 squezeTo;

    [Export]
    public float squishOverTime = 0.25f;

    private Vector3 _unsquishedScale;

    public override void _Ready()
    {
        _unsquishedScale = nodeToSquish.Scale;

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

        Activate();
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

        Deactivate();
    }

    public virtual void Activate()
    {
        _triggerable.Activate();
        CreateTween().TweenProperty(nodeToSquish, "scale", squezeTo, squishOverTime);
    }

    public virtual void Deactivate()
    {
        _triggerable.Deactivate();
        CreateTween().TweenProperty(nodeToSquish, "scale", _unsquishedScale, squishOverTime);
    }
}
