using Godot;

namespace ProjectA.Game.Pickups;

public interface IPickupReaction
{
    void Emit(PickupableArea3D pickupable, Node3D picker);
}
