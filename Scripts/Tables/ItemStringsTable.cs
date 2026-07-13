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
            name = "Coin",
            inventoryListing = "Coin",
            format = "{0}: x{1}",
        },
        [ItemType.Chest] = new()
        {
            name = "Chest",
            inventoryListing = "Chest",
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
