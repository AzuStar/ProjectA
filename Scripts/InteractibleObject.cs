using Godot;

namespace ProjectA.Game;

public abstract partial class IntractibleObject : Area3D
{
    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }

    public abstract void OnBodyEntered(Node3D body);
    public abstract void OnBodyExited(Node3D body);
}
