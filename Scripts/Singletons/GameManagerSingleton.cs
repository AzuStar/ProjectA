using Godot;
using Godot.Collections;
using ProjectA.Game.Levels;

namespace ProjectA.Game.Singletons;

public partial class GameManagerSingleton : Node
{
    public static GameManagerSingleton Instance { get; private set; }

    [Export]
    public int currentLevel = -1;

    [Export]
    public Array<PackedScene> gameLevels = new();

    [Export]
    public float delayBeforeLevelIsShown = 0.2f;

    private static LevelInstance _currentLevelInstance;

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

    public static void MoveToNextLevel()
    {
        UnloadCurrentLevel();

        _currentLevelInstance = LoadLevelInstance(++Instance.currentLevel);
        Bootstrap.GetGameSvc().AddChild(_currentLevelInstance);
    }

    public static void UpdateCurrentLevel(int levelId)
    {
        Instance.currentLevel = levelId;
    }

    private static LevelInstance LoadLevelInstance(int levelId)
    {
        if (levelId > Instance.gameLevels.Count || levelId < 0)
            return null;

        var instance = Instance.gameLevels[levelId].Instantiate<LevelInstance>();

        instance.levelId = levelId;
        return instance;
    }

    private static void UnloadCurrentLevel()
    {
        if (_currentLevelInstance != null)
        {
            _currentLevelInstance.QueueFree();
            _currentLevelInstance = null;
        }
    }

    public static void ReloadCurrentLevel()
    {
        UnloadCurrentLevel();
        _currentLevelInstance = LoadLevelInstance(Instance.currentLevel);
        Bootstrap.GetGameSvc().AddChild(_currentLevelInstance);
    }
}
