using Godot;

namespace ProjectA.Game.Pickups;

public partial class PickupSoundEffect : Node, IPickupReaction
{
    [Export]
    public AudioStream _collectionEffect;

    public void Emit(PickupableArea3D pickupable, Node3D picker)
    {
        if (_collectionEffect != null && AudioPlayerSingleton.Instance != null)
        {
            AudioPlayerSingleton.Instance.PlaySfx(_collectionEffect, pickupable.GlobalPosition);
        }
    }
}
