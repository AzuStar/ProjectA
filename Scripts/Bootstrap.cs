using System.Collections.Generic;
using System.Threading;
using Godot;
using Godot.Collections;
using ProjectA.Game.Registry;

namespace ProjectA.Game;

public partial class Bootstrap : Node
{
    // any logic for saving / loading / initialization
    public static Bootstrap Instance { get; private set; }

    [Export]
    public SubViewportContainer gameSvc;

    [Export]
    public SubViewportContainer uiSvc;

    public static TypeKeyedRegistry registry = new TypeKeyedRegistry();
    private double _registrySaveTimeout = 0;

    /// <summary>
    /// how many menus currently locking the game (free cursor and paused game SVC)
    /// </summary>
    public HashSet<Node> gameLocks = new HashSet<Node>();

    public static void SetGamePausedState(bool locked)
    {
        if (locked)
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
            GetGameSvc().ProcessMode = ProcessModeEnum.Disabled;
        }
        else
        {
            GetGameSvc().ProcessMode = ProcessModeEnum.Inherit;
            Input.MouseMode = Input.MouseModeEnum.Captured;
        }
    }

    public static void LockGameLock(Node whoIsLocking)
    {
        Instance.gameLocks.Add(whoIsLocking);
        if (Instance.gameLocks.Count == 1)
            SetGamePausedState(true);
    }

    public static void ReleaseGameLock(Node whoIsReleasing)
    {
        Instance.gameLocks.Remove(whoIsReleasing);
        if (Instance.gameLocks.Count == 0)
            SetGamePausedState(false);
    }

    public static SubViewport GetGameSvc() => Instance.gameSvc.GetChild<SubViewport>(0);

    public static SubViewport GetUiSvc() => Instance.uiSvc.GetChild<SubViewport>(0);

    public override void _Process(double delta)
    {
        base._Process(delta);

        _registrySaveTimeout -= delta;
        if (_registrySaveTimeout <= 0)
        {
            _registrySaveTimeout = 1;
            SaveRegistry();
        }
    }

    private static void SaveRegistry()
    {
        // todo implement godot save service when we will have persistent data, like highscores etc
        // registry.SaveDirtyKVsOnly(saveService);
    }

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
