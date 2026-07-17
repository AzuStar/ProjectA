using Godot;
using ProjectA.Game.Player;
using ProjectA.Game.Singletons;
using ProjectA.Game.Tables;
using ProjectA.Game.UI;

namespace ProjectA.Game.Interaction;

[GlobalClass]
public partial class LeverSwitch : Node3D
{
    [Export]
    public Area3D interactionArea;

    [Export]
    public Triggerable triggerable;

    [Export]
    public DuoFilter duoFilter = DuoFilter.BOTH;

    [Export]
    public PackedScene popupPrefab;

    private bool _isOn;
    private bool _isInteractable;
    private UiInteractiveFloatingPopup _popup;

    public override void _Process(double delta)
    {
        bool isInteractableNow = ActiveDuoPartIsInRange();

        if (isInteractableNow == _isInteractable)
            return;

        _isInteractable = isInteractableNow;

        if (_isInteractable)
            BecomesInteractable();
        else
            BecomesNotInteractable();
    }

    public override void _Input(InputEvent @event)
    {
        if (
            !@event.IsActionPressed(InputsTable.ACTION_INTERACT)
            || @event is InputEventKey keyEvent && keyEvent.IsEcho()
            || !ActiveDuoPartIsInRange()
        )
            return;

        _isOn = !_isOn;

        if (_isOn)
            Activated();
        else
            Deactivated();
    }

    public override void _ExitTree()
    {
        BecomesNotInteractable();
    }

    private bool ActiveDuoPartIsInRange()
    {
        Godot.Collections.Array<Node3D> bodies = interactionArea.GetOverlappingBodies();
        DuoTarget activePart = PlayerSingleton.Instance.playerDuo.currentlyActivePart;

        for (int i = 0; i < bodies.Count; i++)
        {
            if (CanUse(bodies[i], activePart))
                return true;
        }

        return false;
    }

    private bool CanUse(Node3D body, DuoTarget activePart)
    {
        return activePart == DuoTarget.PLAYER
            ? body is PlayerCharacterController && (duoFilter & DuoFilter.PLAYER) != 0
            : body is DroneCharacterController && (duoFilter & DuoFilter.DRONE) != 0;
    }

    protected virtual void Activated()
    {
        triggerable.Activate();
    }

    protected virtual void Deactivated()
    {
        triggerable.Deactivate();
    }

    protected virtual void BecomesInteractable()
    {
        if (_popup != null)
            return;

        _popup = popupPrefab.Instantiate<UiInteractiveFloatingPopup>();
        _popup.positionSync.syncFrom = this;
        UiRootSingleton.Instance.AddChild(_popup);
    }

    protected virtual void BecomesNotInteractable()
    {
        if (_popup == null)
            return;

        _popup.QueueFree();
        _popup = null;
    }
}
