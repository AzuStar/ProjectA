using System.Collections.Generic;
using ProjectA.Game.Inventory;

namespace ProjectA.Game.Tables;

public static class ItemStringsTable
{
    public static Dictionary<ItemType, ItemStrings> STRINGS = new()
    {
        [ItemType.Key] = new()
        {
            name = "Key",
            inventoryListing = "Keys",
            format = "{0}: x{1}",
        },
        [ItemType.Coin] = new()
        {
            name = "Fruit",
            inventoryListing = "Fruits",
            format = "{0}: x{1}",
        },
        [ItemType.Chest] = new()
        {
            name = "Chest",
            inventoryListing = "Chests",
            format = "{0}: x{1}",
        },
    };
}

public class ItemStrings
{
    public string name;
    public string inventoryListing;
    public string format;
}
