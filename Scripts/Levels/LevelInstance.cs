using Godot;
using ProjectA.Game;
using ProjectA.Game.Inventory;
using ProjectA.Game.Player;
using ProjectA.Game.Registry;
using ProjectA.Game.Singletons;
using ProjectA.Game.UI;
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
    public float starTimeTreshold;

    [Export]
    public int starCoinCollectedTreshold;

    [Export]
    public int starChestsCollectedTreshold;

    public LevelInventory inventory = new();

    private PlayerDroneDuo _duo;
    private ulong _startedAt;

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
        _startedAt = Time.GetTicksMsec();
        UiRootSingleton.Instance.deathMenu.Hide();
        completionArea.BodyEntered += OnLevelComplete;
        UiRootSingleton.Instance.levelMenu.Show();
        _duo = PlayerSingleton.AcquireDuo();

        PrepareDuo(_duo);
        SetGameState(GameState.Playing);
    }

    public override void _EnterTree()
    {
        GameManagerSingleton.UpdateCurrentLevel(levelId);
        UiRootSingleton.Instance.levelMenu.ClearInventory();
    }

    public override void _ExitTree()
    {
        UiRootSingleton.Instance.levelMenu.ClearInventory();
    }

    public override void _Input(InputEvent ev)
    {
        if (
            _currentGameState == GameState.Dead
            && ev is InputEventKey eventKey
            && eventKey.Pressed
            && eventKey.Keycode == Key.R
        )
        {
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
                UiRootSingleton.Instance.deathMenu.Hide();
                break;
            case GameState.Dead:
                _duo.Kill();
                UiRootSingleton.Instance.deathMenu.Show();
                break;
        }

        _currentGameState = newState;

        GD.Print("New game state: " + _currentGameState);
    }

    public void AddItem(ItemType type)
    {
        inventory.Add(type, 1);
        UiRootSingleton.Instance.levelMenu.UpdateInventory(inventory);
    }

    public bool RemoveOneItem(ItemType itemType)
    {
        bool removed = inventory.RemoveOne(itemType);
        UiRootSingleton.Instance.levelMenu.UpdateInventory(inventory);
        return removed;
    }

    public bool HasItem(ItemType itemType)
    {
        return inventory.Has(itemType);
    }

    protected void PrepareDuo(PlayerDroneDuo duo)
    {
        duo.MoveToParent(this);
        duo.Prepare(spawnPoint.GlobalPosition);
    }

    private void OnLevelComplete(Node3D body)
    {
        if (body is not PlayerCharacterController player)
            return;

        SaveLevelStars();
        GD.Print("Moving To Next Level " + GameManagerSingleton.Instance.currentLevel);
        GameManagerSingleton.MoveToNextLevel();
    }

    private void SaveLevelStars()
    {
        float timeSpent = (Time.GetTicksMsec() - _startedAt) / 1000.0f;
        LevelSave levelSave = new()
        {
            timeStar = timeSpent < starTimeTreshold,
            coinsStar = inventory.items[(int)ItemType.Coin].quantity >= starCoinCollectedTreshold,
            chestsStar = inventory.items[(int)ItemType.Chest].quantity >= starChestsCollectedTreshold,
        };

        Bootstrap.registry.GetOrCreate<PlayerSaveRegistryV1>().SaveLevelStars(levelId, levelSave);
    }
}
