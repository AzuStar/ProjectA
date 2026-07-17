using Godot;
using ProjectA.Game.Player;
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
    public Node3D syncFrom;

    private UiFloatingPopup _popup;

    public override void _Ready()
    {
        area.BodyEntered += HandleAreaChanged;
        area.BodyExited += HandleAreaChanged;
    }

    public override void _Process(double delta)
    {
        SetPopupVisible(HasUsableDuoPartInArea());
    }

    public override void _ExitTree()
    {
        HidePopup();
    }

    public bool HasUsableDuoPartInArea()
    {
        Godot.Collections.Array<Node3D> bodies = area.GetOverlappingBodies();

        for (int i = 0; i < bodies.Count; i++)
        {
            if (CanUse(bodies[i]))
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
        if (_popup != null)
            return;

            GD.Print("Showing Popup");

        _popup = popupPrefab.Instantiate<UiFloatingPopup>();
        _popup.positionSync.syncFrom = syncFrom ?? GetParent<Node3D>();
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
        SetPopupVisible(HasUsableDuoPartInArea());
    }

    private bool CanUse(Node3D body)
    {
        if (body is PlayerCharacterController)
            return (duoFilter & DuoFilter.PLAYER) != 0;

        if (body is DroneCharacterController)
            return (duoFilter & DuoFilter.DRONE) != 0;

        return false;
    }
}
