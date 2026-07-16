using Godot;
using ProjectA.Game.Singletons;

namespace ProjectA.Game.UI;

public partial class UiCreditsMenu : Control
{
    public override void _Input(InputEvent @event)
    {
        if (!Visible || @event is not InputEventMouseButton { Pressed: true })
            return;

        GameManagerSingleton.BackToMainMenu();
    }

    public new void Show()
    {
        base.Show();
        Bootstrap.LockGameLock(this);
    }

    public new void Hide()
    {
        base.Hide();
        Bootstrap.ReleaseGameLock(this);
    }
}
