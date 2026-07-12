using Godot;
using ProjectA.Game.Singletons;

namespace ProjectA.Game.UI;

public partial class UiMainMenu : Control
{
    [Export]
    public Button playLevel0;

    public override void _Ready()
    {
        base._Ready();
        Show(); // we want main menu to always show up even if someone messed up settings in editor

        playLevel0.Pressed += GameManagerSingleton.MoveToNextLevel;
    }

    public new void Hide()
    {
        Bootstrap.ReleaseGameLock(this);
        base.Hide();
    }

    public new void Show()
    {
        base.Show();
        Bootstrap.LockGameLock(this);
    }
}
