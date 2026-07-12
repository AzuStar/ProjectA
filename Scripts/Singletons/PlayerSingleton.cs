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

    [Export]
    public float MouseSensitivity = 0.0025f;

    [Export]
    public float maxDroneLeashRange = 10.0f;

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

    // pass input from root node through svc
    public override void _Input(InputEvent @event)
    {
        playerDuo.HandleInput(@event, MouseSensitivity);
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
    }

    public static PlayerDroneDuo AcquireDuo()
    {
        return Instance.playerDuo;
    }
}
