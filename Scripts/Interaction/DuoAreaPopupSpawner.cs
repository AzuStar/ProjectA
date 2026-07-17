using Godot;
using ProjectA.Game.Player;
using ProjectA.Game.Singletons;
using ProjectA.Game.UI;

namespace ProjectA.Game.Interaction;

[GlobalClass]
public partial class DuoAreaPopupSpawner : Node
{
    [Export]
    public Area3D area;

    [Export]
    public DuoFilter duoFilter = DuoFilter.BOTH;

    [Export]
    public PackedScene popupPrefab;

    [Export]
    public string popupText = "Press E";

    [Export]
    public Node3D syncFrom;

    [Export]
    public bool autoShow = true;

    [Export]
    public bool requireActiveDuoPart;

    private UiInteractiveFloatingPopup _popup;

    public override void _Ready()
    {
        if (area != null)
        {
            area.BodyEntered += HandleAreaChanged;
            area.BodyExited += HandleAreaChanged;
        }
    }

    public override void _Process(double delta)
    {
        if (!autoShow)
            return;

        SetPopupVisible(HasUsableDuoPartInArea(requireActiveDuoPart));
    }

    public override void _ExitTree()
    {
        HidePopup();
    }

    public bool HasUsableDuoPartInArea(bool activePartOnly)
    {
        if (area == null)
            return false;

        Godot.Collections.Array<Node3D> bodies = area.GetOverlappingBodies();
        DuoTarget activePart = activePartOnly
            ? PlayerSingleton.Instance.playerDuo.currentlyActivePart
            : default;

        for (int i = 0; i < bodies.Count; i++)
        {
            if (CanUse(bodies[i], activePart, activePartOnly))
                return true;
        }

        return false;
    }

    public void SetPopupVisible(bool visible)
    {
        if (visible)
            ShowPopup();
        else
            HidePopup();
    }

    public void ShowPopup()
    {
        if (_popup != null || popupPrefab == null)
            return;

        _popup = popupPrefab.Instantiate<UiInteractiveFloatingPopup>();
        _popup.positionSync.syncFrom = syncFrom ?? GetParent<Node3D>();
        _popup.SetText(popupText);
        UiRootSingleton.Instance.AddChild(_popup);
    }

    public void HidePopup()
    {
        if (_popup == null)
            return;

        _popup.QueueFree();
        _popup = null;
    }

    private void HandleAreaChanged(Node3D body)
    {
        if (autoShow)
            SetPopupVisible(HasUsableDuoPartInArea(requireActiveDuoPart));
    }

    private bool CanUse(Node3D body, DuoTarget activePart, bool activePartOnly)
    {
        if (body is PlayerCharacterController)
            return (duoFilter & DuoFilter.PLAYER) != 0 && (!activePartOnly || activePart == DuoTarget.PLAYER);

        if (body is DroneCharacterController)
            return (duoFilter & DuoFilter.DRONE) != 0 && (!activePartOnly || activePart == DuoTarget.DRONE);

        return false;
    }
}
