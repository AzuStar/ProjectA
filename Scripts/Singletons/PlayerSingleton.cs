using Godot;
using ProjectA.Game.Player;
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
    public PlayerDroneDuo playerDuo;

    private bool _isPlayerDuoPrepared;

    private int _coinsCollected;

    public int CoinsCollected
    {
        get => _coinsCollected;
        set
        {
            if (_coinsCollected == value)
                return;

            _coinsCollected = value;
            UpdateText();
        }
    }

    public int BaxPattedTimes;

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

    public void UpdateText()
    {
        UiRootSingleton.Instance?.firstLabel.Text = CompileText();
    }

    public string CompileText() =>
        $"""
            COINS PICKED UP: {CoinsCollected}
            PATTED BAX: {BaxPattedTimes} times
            """;

    /// <summary>
    /// Releases the duo into singleton custody
    /// </summary>
    public static void ReleaseTheDuo()
    {
        Instance.playerDuo.MoveToParent(Instance);
        Instance.playerDuo.player.GlobalPosition = Vector3.Zero;
        Instance.playerDuo.drone.GlobalPosition = Vector3.Zero;
    }

    public static PlayerDroneDuo AcquireDuo()
    {
        return Instance.playerDuo;
    }
}
