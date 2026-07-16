using Godot;

namespace ProjectA.Game.UI;

public partial class UiRootSingleton : Control
{
    public static UiRootSingleton Instance { get; private set; }

    [Export]
    public UiLevelMenu levelMenu;

    [Export]
    public UiEscMenu escMenu;

    [Export]
    public UiMainMenu mainMenu;

    [Export]
    public UiDeathMenu deathMenu;

    [Export]
    public UiCreditsMenu creditsMenu;

    public override void _EnterTree()
    {
        Instance = this;
    }

    public override void _ExitTree()
    {
        if (Instance == this)
            Instance = null;
    }
}
