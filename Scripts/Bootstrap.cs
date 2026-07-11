using Godot;
using Godot.Collections;

namespace ProjectA.Game;

public partial class Bootstrap : Node
{
    // any logic for saving / loading / initialization
    public static Bootstrap Instance { get; private set; }

    [Export]
    public SubViewportContainer gameSvc;

    [Export]
    public SubViewportContainer uiSvc;

    public static SubViewport GetGameSvc() => Instance.gameSvc.GetChild<SubViewport>(0);

    public static SubViewport GetUiSvc() => Instance.uiSvc.GetChild<SubViewport>(0);

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
