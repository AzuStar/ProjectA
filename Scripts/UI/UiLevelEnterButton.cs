using Godot;
using ProjectA.Game.Levels;
using ProjectA.Game.Registry;
using ProjectA.Game.Singletons;

namespace ProjectA.Game.UI;

public partial class UiLevelEnterButton : Button
{
    [Export]
    public LevelEnum levelId;

    [Export]
    public RichTextLabel label;

    [Export]
    public TextureRect timeStar;

    [Export]
    public TextureRect coinStar;

    [Export]
    public TextureRect chestStar;

    public override void _Ready()
    {
        Pressed += EnterLevel;
        UpdateVisuals();
    }

    public override void _Notification(int what)
    {
        if (what == NotificationVisibilityChanged && IsVisibleInTree())
            UpdateVisuals();
    }

    private void EnterLevel()
    {
        GameManagerSingleton.MoveToLevel(levelId);
    }

    private void UpdateVisuals()
    {
        label.Text = $"[b]ENTER {levelId}[/b]";

        bool hasCompletedPreviousLevel;
        if (levelId > LevelEnum.LEVEL1)
        {
            bool gotPreviousLevelSave = Bootstrap.registry.GetOrCreate<PlayerSaveRegistryV1>().GetLevelSave(levelId - 1, out LevelSave previousLevelSave);
            hasCompletedPreviousLevel = gotPreviousLevelSave && previousLevelSave.cleared;
        }
        else
        {
            // There is no level before this one.
            hasCompletedPreviousLevel = true;
        }
        Disabled = !hasCompletedPreviousLevel;

        bool gotLevelSave = Bootstrap.registry.GetOrCreate<PlayerSaveRegistryV1>().GetLevelSave(levelId, out LevelSave levelSave);

        if (gotLevelSave)
        {
            if (levelSave.timeStar)
                timeStar.SelfModulate = Colors.White;

            if (levelSave.coinsStar)
                coinStar.SelfModulate = Colors.White;

            if (levelSave.chestsStar)
                chestStar.SelfModulate = Colors.White;
        }
    }
}
