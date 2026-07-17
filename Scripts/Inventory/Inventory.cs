using System;
using System.Collections.Generic;
using System.Text;
using Godot;
using ProjectA.Game.Tables;

namespace ProjectA.Game.Inventory;

public class LevelInventory
{
    public Item[] items = new Item[(int)ItemType.COUNT];

    public LevelInventory()
    {
        for (int i = 0; i < items.Length; i++)
            items[i] = new Item() { itemType = (ItemType)i };
    }

    public void Add(ItemType type, int quantity)
    {
        items[(int)type].quantity += quantity;
    }

    public bool Has(ItemType itemType)
    {
        if (items[(int)itemType].quantity > 0)
            return true;

        return false;
    }

    public bool RemoveOne(ItemType itemType)
    {
        if (items[(int)itemType].quantity > 0)
        {
            items[(int)itemType].quantity--;
            return true;
        }
        return false;
    }

    public void Clear()
    {
        Array.Clear(items, 0, items.Length);
    }

    public string CompileInventoryText()
    {
        StringBuilder sb = new();
        for (int i = 0; i < items.Length; i++)
        {
            var item = items[i];
            if (item.quantity == 0)
                continue;
            var itemString = ItemStringsTable.STRINGS[item.itemType];
            sb.AppendFormat(itemString.format, itemString.inventoryListing, item.quantity);
            sb.AppendLine();
        }

        return sb.ToString();
    }
}
