using Godot;

namespace ProjectA.Game.Inventory;

public abstract class Item
{
    public abstract ItemType ItemType { get; }

    public int quantity = 1;

    public virtual string DisplayText()
    {
        return $"{ItemType, -20} x{quantity}";
    }
}
