using System.Diagnostics;
using Godot;
using ProjectA.Game.Interaction;

namespace ProjectA.Game.Barriers;

public abstract partial class Barrier : Triggerable
{
    [Export]
    public PhysicsBody3D collisionObject;

    [Export]
    public Node3D visualRoot;

    private uint _openCount;
    private uint _originalCollisionLayer;

    private bool IsOpen => _openCount > 0;

    public override void _Ready()
    {
        base._Ready();
        _originalCollisionLayer = collisionObject.CollisionLayer;
    }

    public virtual bool TryOpen()
    {
        try
        {
            if (IsOpen)
                return false;

            collisionObject.CollisionLayer = 0;
            visualRoot.Visible = false;

            return true;
        }
        finally
        {
            // Still count open attempts even when we fail,
            // so competing actions hold the barrier open
            _openCount++;
        }
    }

    public virtual bool TryClose()
    {
        if (!IsOpen)
            return false;

        _openCount--;

        if (!IsOpen)
        {
            // We were the last source holding it open
            collisionObject.CollisionLayer = _originalCollisionLayer;
            visualRoot.Visible = true;

            return true;
        }

        return false;
    }

    public override void Activate()
    {
        TryOpen();
    }

    public override void Deactivate()
    {
        TryClose();
    }
}
