using Godot;
using ProjectA.Game.Player;
using ProjectA.Game.Tables;
using ProjectA.Game.UI;
using ProjectA.Game.Utils;

namespace ProjectA.Game.Singletons;

/// <summary>
/// Contains persistent player state. PlayerDroneDuo is stateless.
/// </summary>
public partial class PlayerSingleton : Node
{
    // this needs to be guaranteed to exist for the game to function
    public static PlayerSingleton Instance { get; private set; }

    [Export]
    public PackedScene duoToUse;

    /// <summary>
    /// Dont modify this
    /// </summary>
    [Export]
    public PlayerDroneDuo playerDuo;

    public override void _EnterTree()
    {
        if (Instance != null && Instance != this)
        {
            QueueFree();
            return;
        }

        Instance = this;
    }

    public override void _ExitTree()
    {
        if (Instance == this)
            Instance = null;
    }

    public override void _Ready()
    {
        UpdateText();
    }

    // pass input from root node through svc
    public override void _Input(InputEvent @event)
    {
        if (playerDuo == null)
            return;

        playerDuo.HandleInput(@event, cameraMouseSensitivity);
    }

    public void UpdateText() { }

    public static PlayerDroneDuo AcquireDuo()
    {
        if (Instance.playerDuo != null)
        {
            Instance.playerDuo = null;
        }

        Instance.playerDuo = Instance.duoToUse.Instantiate<PlayerDroneDuo>();
        Instance.AddChild(Instance.playerDuo);

        return Instance.playerDuo;
    }
}
