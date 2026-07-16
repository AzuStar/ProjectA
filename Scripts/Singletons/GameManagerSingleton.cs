using Godot;
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
    public LevelEnum currentLevel;

    [Export]
    public Godot.Collections.Dictionary<LevelEnum, PackedScene> gameLevels = new();

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
        LevelEnum nextLevel = (LevelEnum)((int)Instance.currentLevel + 1);
        if (HasLevelInstance(nextLevel))
        {
            MoveToLevel(nextLevel);
            return;
        }

        ShowCreditsMenu();
    }

    public static void MoveToLevel(LevelEnum levelId)
    {
        UiRootSingleton.Instance.mainMenu.Hide();
        UiRootSingleton.Instance.creditsMenu.Hide();
        UnloadCurrentLevel();
        OpenLevel(LoadLevelInstance(levelId));
    }

    public static void UpdateCurrentLevel(LevelEnum levelId)
    {
        Instance.currentLevel = levelId;
    }

    private static bool HasLevelInstance(LevelEnum levelId)
    {
        return Instance.gameLevels.ContainsKey(levelId);
    }

    private static LevelInstance LoadLevelInstance(LevelEnum levelId)
    {
        var instance = Instance.gameLevels[levelId].Instantiate<LevelInstance>();
        instance.levelId = levelId;
        return instance;
    }

    private static void OpenLevel(LevelInstance level)
    {
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
        UiRootSingleton.Instance.escMenu.Hide();
        UiRootSingleton.Instance.levelMenu.Hide();
        UiRootSingleton.Instance.mainMenu.Show();
        UiRootSingleton.Instance.deathMenu.Hide();
        UiRootSingleton.Instance.creditsMenu.Hide();
    }

    private static void ShowCreditsMenu()
    {
        UnloadCurrentLevel();
        UiRootSingleton.Instance.escMenu.Hide();
        UiRootSingleton.Instance.levelMenu.Hide();
        UiRootSingleton.Instance.mainMenu.Hide();
        UiRootSingleton.Instance.deathMenu.Hide();
        UiRootSingleton.Instance.creditsMenu.Show();
    }
}
