using Godot;
using Godot.Collections;
using ProjectA.Game.Levels;
using ProjectA.Game.UI;

namespace ProjectA.Game.Singletons;

/// <summary>
/// Contains all level management.
/// </summary>
public partial class GameManagerSingleton : Node
{
    public static GameManagerSingleton Instance { get; private set; }

    [Export]
    public int currentLevel = -1;

    [Export]
    public Array<PackedScene> gameLevels = new();

    [Export]
    public float delayBeforeLevelIsShown = 0.2f;

    public static LevelInstance currentLevelInstance;

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
        MoveToLevel(++Instance.currentLevel);
    }

    public static void MoveToLevel(int levelId)
    {
        UiRootSingleton.Instance.mainMenu.Hide();
        UnloadCurrentLevel();
        OpenLevel(LoadLevelInstance(levelId));
    }

    public static void UpdateCurrentLevel(int levelId)
    {
        Instance.currentLevel = levelId;
    }

    private static LevelInstance LoadLevelInstance(int levelId)
    {
        if (levelId > Instance.gameLevels.Count || levelId < 0)
        {
            GD.PrintErr($"Level {levelId} doesn't exist.");
            return null;
        }

        var instance = Instance.gameLevels[levelId].Instantiate<LevelInstance>();

        instance.levelId = levelId;
        return instance;
    }

    private static void OpenLevel(LevelInstance level)
    {
        if (level == null)
        {
            return;
        }

        currentLevelInstance = level;
        Bootstrap.GetGameSubViewport().AddChild(currentLevelInstance);
    }

    private static bool UnloadCurrentLevel()
    {
        if (currentLevelInstance == null)
            return false;

        currentLevelInstance.QueueFree();
        currentLevelInstance = null;
        return true;
    }

    public static void ReloadCurrentLevel()
    {
        UnloadCurrentLevel();
        OpenLevel(LoadLevelInstance(Instance.currentLevel));
    }

    public static void BackToMainMenu()
    {
        UnloadCurrentLevel();
        Instance.currentLevel = -1;
        UiRootSingleton.Instance.escMenu.Hide();
        UiRootSingleton.Instance.levelMenu.Hide();
        UiRootSingleton.Instance.mainMenu.Show();
        UiRootSingleton.Instance.deathMenu.Hide();
    }
}
