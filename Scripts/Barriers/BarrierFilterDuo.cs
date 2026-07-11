using Godot;
using ProjectA.Game.Player;

namespace ProjectA.Game.Barriers;

public enum BarrierDuoFilter
{
    Player,
    Drone,
}

public partial class BarrierFilterDuo : Barrier
{
    [Export]
    public Area3D filterArea;

    [Export]
    public BarrierDuoFilter letsThrough;

    public override void _Ready()
    {
        filterArea.BodyEntered += OnBodyEntered;
        filterArea.BodyExited += OnBodyExited;
    }

    private void OnBodyEntered(Node3D body)
    {
        if (CanPass(body))
            collisionObject.AddCollisionExceptionWith(body);
    }

    private void OnBodyExited(Node3D body)
    {
        if (CanPass(body))
            collisionObject.RemoveCollisionExceptionWith(body);
    }

    private bool CanPass(Node3D body)
    {
        return letsThrough == BarrierDuoFilter.Player
            ? body is PlayerCharacterController
            : body is DroneCharacterController;
    }
}
