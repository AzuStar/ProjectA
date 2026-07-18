using Godot;

namespace ProjectA.Game.Pickups;

public partial class PickupSoundEffect : Node, IPickupReaction
{
    [Export]
    public AudioStream[] _collectionEffects;

    public void Emit(PickupableArea3D pickupable, Node3D picker)
    {
        if (_collectionEffects != null && _collectionEffects.Length > 0 && AudioPlayerSingleton.Instance != null)
        {
            AudioStream chosenEffect = _collectionEffects[GD.RandRange(0, _collectionEffects.Length - 1)];
            AudioPlayerSingleton.Instance.PlaySfx(chosenEffect, pickupable.GlobalPosition);
        }
    }
}
