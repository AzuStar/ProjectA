using Godot;

namespace ProjectA.Game.Barriers;

public abstract partial class Barrier : Node3D
{
    [Export]
    public PhysicsBody3D collisionObject;

    [Export]
    public Node3D visualRoot;

    protected bool isOpen;

    public virtual void Open()
    {
        isOpen = true;
        collisionObject.CollisionLayer = 0;
        visualRoot.Visible = false;
    }
}
