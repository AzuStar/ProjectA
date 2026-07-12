using Godot;
using ProjectA.Game.Inventory;
using ProjectA.Game.Singletons;

namespace ProjectA.Game.UI;

public partial class UiLevelMenu : Control
{
    [Export]
    public RichTextLabel persistentState;

    [Export]
    public RichTextLabel inventoryLabel;

    public override void _Ready()
    {
        base._Ready();
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
