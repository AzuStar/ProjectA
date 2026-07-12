using System.Collections.Generic;
using ProjectA.Game.Inventory;

namespace ProjectA.Game.Tables;

public static class ItemStringsTable
{
    public static Dictionary<ItemType, ItemStrings> STRINGS = new()
    {
        [ItemType.Key] = new() { Name = "Key", InventoryListing = "Keys" },
    };
}

public class ItemStrings
{
    public string Name;
    public string InventoryListing;
}
