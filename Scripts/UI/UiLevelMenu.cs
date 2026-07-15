using Godot;
using ProjectA.Game.Inventory;
using ProjectA.Game.Singletons;

namespace ProjectA.Game.UI;

public partial class UiLevelMenu : Control
{
    [Export]
    public RichTextLabel stateLabel;

    [Export]
    public RichTextLabel inventoryLabel;

    [Export]
    public TextureProgressBar droneSkill;

    public void UpdateInventory(LevelInventory inventory)
    {
        inventoryLabel.Text = inventory.CompileInventoryText();
    }

    public void ClearInventory()
    {
        inventoryLabel.Text = string.Empty;
    }

    public void UpdateTime(double elapsedTime)
    {
        int minutes = (int)(elapsedTime / 60);
        int seconds = (int)elapsedTime % 60;
        int milliseconds = (int)((elapsedTime - (int)elapsedTime) * 1000);
        stateLabel.Text = $"{minutes:00}:{seconds:00}.{milliseconds:000}";
    }

    public void UpdateDroneCooldown(float remaining, float duration)
    {
        droneSkill.MaxValue = Mathf.Max(1.0f, duration);
        droneSkill.Value = remaining;
    }
}
