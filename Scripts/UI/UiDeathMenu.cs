using System.Diagnostics.Contracts;
using Godot;
using ProjectA.Game.Singletons;

namespace ProjectA.Game.UI;

public partial class UiDeathMenu : Control
{
    [Export]
    public Button backtoMainMenu;

    [Export]
    public Button restartLevelButton;

    public override void _Ready()
    {
        base._Ready();
        backtoMainMenu.Pressed += GameManagerSingleton.BackToMainMenu;
        restartLevelButton.Pressed += GameManagerSingleton.ReloadCurrentLevel;
    }

    public new void Show()
    {
        base.Show();
        backtoMainMenu.Visible = true;
        Bootstrap.LockGameLock(this);
    }

    public new void Hide()
    {
        base.Hide();
        backtoMainMenu.Visible = false;
        Bootstrap.ReleaseGameLock(this);
    }
}
