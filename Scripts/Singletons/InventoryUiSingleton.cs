using Godot;
using ProjectA.Game.Inventory;

namespace ProjectA.Game.Singletons;

public partial class InventoryUiSingleton : Control
{
    public static InventoryUiSingleton Instance { get; private set; }

    [Export]
    public RichTextLabel inventoryLabel;

    public override void _EnterTree()
    {
        if (Instance != null && Instance != this)
        {
            QueueFree();
            return;
        }

        Instance = this;
    }

    public override void _ExitTree()
    {
        if (Instance == this)
            Instance = null;
    }

    public void UpdateInventory(LevelInventory inventory)
    {
        string[] lines = new string[inventory.items.Count];
        for (int i = 0; i < inventory.items.Count; i++)
            lines[i] = inventory.items[i].DisplayText();

        inventoryLabel.Text = string.Join("\n", lines);
    }

    public void ClearInventory()
    {
        inventoryLabel.Text = string.Empty;
    }
}
