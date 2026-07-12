using Godot;
using ProjectA.Game;
using ProjectA.Game.Inventory;
using ProjectA.Game.Player;
using ProjectA.Game.Singletons;
using ProjectA.Game.Utils;

namespace ProjectA.Game.Levels;

/// <summary>
/// Contains all level state.
/// </summary>
public partial class LevelInstance : Node3D
{
    // read, but dont set. bish
    public int levelId;

    [Export]
    public Marker3D spawnPoint;

    [Export]
    public Area3D completionArea;

    [Export]
    public RichTextLabel _gameOverLabel;

    public static LevelInstance Current { get; private set; }

    public LevelInventory inventory = new();

    private PlayerDroneDuo _duo;
    public bool IsPlayerDuoPrepared => _duo.IsPrepared;

    public enum GameState
    {
        Undefined,
        Playing,
        Dead,
    }
    private GameState _currentGameState;

    public GameState CurrentGameState
    {
        get => _currentGameState;
        set => SetGameState(value);
    }

    public override void _Ready()
    {
        // PlayerSingleton.
        completionArea.BodyEntered += OnBodyEntered;
        _duo = PlayerSingleton.AcquireDuo();

        PrepareDuo(_duo);
        SetGameState(GameState.Playing);
    }

    public override void _EnterTree()
    {
        Current = this;
        GameManagerSingleton.UpdateCurrentLevel(levelId);
        InventoryUiSingleton.Instance.UpdateInventory(inventory);
    }

    public override void _ExitTree()
    {
        PlayerSingleton.ReleaseTheDuo();
        InventoryUiSingleton.Instance.ClearInventory();
        if (Current == this)
            Current = null;
    }

    public override void _Input(InputEvent ev)
    {
        if (_currentGameState == GameState.Dead && ev is InputEventKey eventKey && eventKey.Pressed && eventKey.Keycode == Key.R)
        {
            TeardownDuo(_duo); // Move singleton player out of the level before unloading it
            GameManagerSingleton.ReloadCurrentLevel();
        }
    }

    private void SetGameState(GameState newState)
    {
        if (newState == _currentGameState)
        {
            return;
        }

        switch (newState)
        {
            case GameState.Undefined:
                break;
            case GameState.Playing:
                break;
            case GameState.Dead:
                _duo.Kill();
                break;
        }
        
        _currentGameState = newState;

        // Temporary, send switch events to a UI handler?
        _gameOverLabel.Visible = newState == GameState.Dead;

        GD.Print("New game state: " + _currentGameState);
    }

    public void AddItem(Item item)
    {
        inventory.Add(item);
        InventoryUiSingleton.Instance.UpdateInventory(inventory);
    }

    public bool RemoveOneItem(ItemType itemType)
    {
        bool removed = inventory.RemoveOne(itemType);
        InventoryUiSingleton.Instance.UpdateInventory(inventory);
        return removed;
    }

    public bool HasItem(ItemType itemType)
    {
        return inventory.Has(itemType);
    }

    public string CompileInventoryText()
    {
        string[] lines = new string[inventory.items.Count];
        for (int i = 0; i < inventory.items.Count; i++)
            lines[i] = inventory.items[i].DisplayText();

        return string.Join("\n", lines);
    }

    protected void PrepareDuo(PlayerDroneDuo duo)
    {
        duo.MoveToParent(this);
        duo.Prepare(spawnPoint.GlobalPosition);
    }

    protected void TeardownDuo(PlayerDroneDuo duo)
    {
        duo.Unprepare();
        duo.MoveToParent(PlayerSingleton.Instance);
    }

    private void OnBodyEntered(Node3D body)
    {
        if (body is not PlayerCharacterController player)
            return;

        PlayerSingleton.ReleaseTheDuo();
        QueueFree();
        GameManagerSingleton.MoveToNextLevel();
    }
}
