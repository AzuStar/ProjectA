using Godot;
using ProjectA.Game;
using ProjectA.Game.Singletons;

namespace ProjectA.Game.UI;

public partial class UiEscMenu : Control
{
    public static UiEscMenu Instance { get; private set; }

    [Export]
    public Button exitButton;

    [Export]
    public Button resumeButton;

    [Export]
    public Button restartLevelButton;

    public override void _EnterTree()
    {
        if (Instance != null && Instance != this)
        {
            QueueFree();
            return;
        }

        Instance = this;
        ProcessMode = ProcessModeEnum.Always;
    }

    public override void _ExitTree()
    {
        if (Instance == this)
            Instance = null;
    }

    public override void _Ready()
    {
        Visible = false;
        resumeButton.Pressed += ResumeGame;
        exitButton.Pressed += ExitGame;
        restartLevelButton.Pressed += GameManagerSingleton.ReloadCurrentLevel;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is not InputEventKey { Pressed: true, Keycode: Key.Escape } keyEvent || keyEvent.IsEcho())
            return;

        if (GameManagerSingleton.currentLevelInstance == null)
            return;

        OpenMenu();
    }

    public void OpenMenu()
    {
        Bootstrap.LockGameLock(this);
        Show();
    }

    public void ResumeGame()
    {
        Bootstrap.ReleaseGameLock(this);
        Hide();
    }

    public void ExitGame()
    {
        GetTree().Quit();
    }
}
