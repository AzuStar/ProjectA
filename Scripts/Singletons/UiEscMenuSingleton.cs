using Godot;
using ProjectA.Game;

namespace ProjectA.Game.Singletons;

public partial class UiEscMenuSingleton : Control
{
    public static UiEscMenuSingleton Instance { get; private set; }

    [Export]
    public Button exitButton;

    [Export]
    public Button resumeButton;

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
        resumeButton.ProcessMode = ProcessModeEnum.Always;
        exitButton.ProcessMode = ProcessModeEnum.Always;
        Visible = false;
        resumeButton.Pressed += ResumeGame;
        exitButton.Pressed += ExitGame;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is not InputEventKey { Pressed: true, Keycode: Key.Escape } keyEvent || keyEvent.IsEcho())
            return;

        OpenMenu();
    }

    public void OpenMenu()
    {
        Visible = true;
        Input.MouseMode = Input.MouseModeEnum.Visible;
        Bootstrap.GetGameSubViewport().ProcessMode = ProcessModeEnum.Disabled;
    }

    public void ResumeGame()
    {
        Visible = false;
        Bootstrap.GetGameSubViewport().ProcessMode = ProcessModeEnum.Inherit;
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public void ExitGame()
    {
        GetTree().Quit();
    }
}
