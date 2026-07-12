using System.Collections.Generic;
using Godot;

namespace ProjectA.Game.Inventory;

public class LevelInventory
{
    public List<Item> items = new();

    public void Add(Item item)
    {
        foreach (Item existing in items)
        {
            if (existing.ItemType != item.ItemType)
                continue;

            existing.quantity += item.quantity;
            return;
        }

        items.Add(item);
    }

    public bool Has(ItemType itemType)
    {
        foreach (Item item in items)
        {
            if (item.ItemType == itemType && item.quantity > 0)
                return true;
        }

        return false;
    }

    public bool RemoveOne(ItemType itemType)
    {
        foreach (Item item in items)
        {
            if (item.ItemType != itemType || item.quantity <= 0)
                continue;

            item.quantity--;
            if (item.quantity == 0)
                items.Remove(item);

            return true;
        }

        return false;
    }
}
