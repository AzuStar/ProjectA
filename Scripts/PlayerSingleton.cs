using Godot;

namespace ProjectA.Game;

public partial class PlayerSingleton : Node
{
    public static PlayerSingleton? Instance { get; private set; }

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
}
