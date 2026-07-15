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

    [Export]
    public PlayerDroneDuo playerDuo;

    private bool _isPlayerDuoPrepared;

    [Export]
    public float MouseSensitivity = 0.0025f;

    [ExportGroup("Drone Leash")]
    [Export]
    public float maxDroneLeashRange = 10.0f;

    [Export]
    public Vector2 droneScreenMaterialCloseDimensions;

    [Export]
    public Vector2 droneScreenMaterialFarDimensions;

    [Export]
    public Curve droneScreenMaterialDimensionsCurve;

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

        playerDuo.HandleInput(@event, MouseSensitivity);
    }

    public void UpdateText()
    {
    }

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
