using Godot;
using ProjectA.Game;
using ProjectA.Game.Inventory;
using ProjectA.Game.Player;
using ProjectA.Game.Singletons;
using ProjectA.Game.Utils;

namespace ProjectA.Game.Levels;

public partial class LevelInstance : Node3D
{
    // read, but dont set. bish
    public int levelId;

    [Export]
    public Marker3D spawnPoint;

    [Export]
    public Area3D completionArea;

    public static LevelInstance Current { get; private set; }

    public LevelInventory inventory = new();

    private PlayerDroneDuo _duo;

    public override async void _Ready()
    {
        // PlayerSingleton.
        completionArea.BodyEntered += OnBodyEntered;
        _duo = PlayerSingleton.AcquireDuo();

        // cheers godot, very cool
        // https://github.com/godotengine/godot/issues/84677
        await ToSignal(GetTree().CreateTimer(0.1), SceneTreeTimer.SignalName.Timeout);
        PrepareDuo(_duo);
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

        Vector3 spawnPosition = NavigationServer3D.MapGetClosestPoint(
            GetWorld3D().NavigationMap,
            spawnPoint.GlobalPosition
        );

        duo.player.GlobalPosition = spawnPosition;
        duo.player.ProcessMode = ProcessModeEnum.Inherit;
        duo.player.acceptInput = true;
        duo.player.DriveCameraSmoothingTarget = true;

        duo.DisableDrone();
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
