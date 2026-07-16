using Godot;

namespace ProjectA.Game.Interaction;

public abstract partial class Triggerable : Node3D
{
    public abstract void Activate();
    public abstract void Deactivate();
}
