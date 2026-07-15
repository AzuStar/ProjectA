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
        inventoryLabel.Text = inventory.CompileInventoryText();
    }

    public void ClearInventory()
    {
        inventoryLabel.Text = string.Empty;
    }
}
