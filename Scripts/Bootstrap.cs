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
    private static GodotSaveService saveService = new();
    private double _registrySaveTimeout = 0;

    /// <summary>
    /// how many menus currently locking the game (free cursor and paused game SVC)
    /// </summary>
    public HashSet<Node> gameLocks = new HashSet<Node>();

    public static bool IsGameLocked => Instance.gameLocks.Count > 0;

    public static void SetGamePausedState(bool locked)
    {
        ProcessModeEnum processMode;

        if (locked)
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
            processMode = ProcessModeEnum.Disabled;
        }
        else
        {
            Input.MouseMode = Input.MouseModeEnum.Captured;
            processMode = ProcessModeEnum.Inherit;
        }

        GetGameSubViewport().SetDeferred(Node.PropertyName.ProcessMode, (int)processMode);
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

    public static SubViewportContainer GetGameSubViewportContainer() => Instance.gameSvc;

    public static SubViewport GetGameSubViewport() => GetGameSubViewportContainer().GetChild<SubViewport>(0);

    public static SubViewportContainer GetUiSubViewportContainer() => Instance.uiSvc;

    public static SubViewport GetUiSubViewport() => GetUiSubViewportContainer().GetChild<SubViewport>(0);

    public override void _Process(double delta)
    {
        base._Process(delta);

        _registrySaveTimeout -= delta;
        if (_registrySaveTimeout <= 0)
        {
            _registrySaveTimeout = 5;
            SaveRegistry();
        }
    }

    private static void SaveRegistry()
    {
        registry.SaveDirtyKVsOnly(saveService);
    }

    public override void _EnterTree()
    {
        Instance = this;
        registry.InitializeRegistry(saveService);
    }

    public override void _ExitTree()
    {
        if (Instance == this)
            Instance = null;
    }
}
