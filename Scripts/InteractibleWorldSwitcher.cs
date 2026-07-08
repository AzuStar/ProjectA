using Godot;

namespace ProjectA.Game;

public partial class InteractibleWorldSwitcher : Area3D
{
    [Export]
    public Node3D? firstNode;

    [Export]
    public Node3D? secondNode;

    [Export]
    public PackedScene? interactPanel;

    [Export]
    public Vector3 interactPanelWorldOffset = new(0.0f, 2.25f, 0.0f);

    private const string PIXEL_ART_SVC_MAT = "uid://8glqgy4sd4vi";

    private bool _playerInRange;
    private bool _isSecondNodeActive;

    private Control? _interactPanelReference;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;

        _isSecondNodeActive = secondNode?.Visible == true && firstNode?.Visible != true;
    }

    public override void _Process(double delta)
    {
        if (_interactPanelReference != null)
            UpdateInteractPanelPosition();
    }

    public override void _ExitTree()
    {
        DespawnInteractPanel();
    }

    private void OnBodyEntered(Node3D body)
    {
        if (body is not PlayerCharacterController)
            return;

        _playerInRange = true;
        SpawnInteractPanel();
    }

    private void OnBodyExited(Node3D body)
    {
        if (body is not PlayerCharacterController)
            return;

        _playerInRange = false;
        DespawnInteractPanel();
    }

    public override void _Input(InputEvent @event)
    {
        if (
            !_playerInRange
            || @event is not InputEventKey { Pressed: true, Keycode: Key.E } keyEvent
            || keyEvent.IsEcho()
        )
            return;

        ToggleWorld();
        _isSecondNodeActive = !_isSecondNodeActive;
        ApplyState();
    }

    private void ApplyState()
    {
        SetNodeActive(firstNode, !_isSecondNodeActive);
        SetNodeActive(secondNode, _isSecondNodeActive);
    }

    private void ToggleWorld()
    {
        if (Bootstrap.Instance?.gameSvc.Material is not ShaderMaterial pixelMaterial)
        {
            return;
        }

        bool current = pixelMaterial.GetShaderParameter("use_palette").AsBool();
        pixelMaterial.SetShaderParameter("use_palette", !current);
    }

    private void SpawnInteractPanel()
    {
        if (_interactPanelReference != null)
            return;

        UiRootSingleton? uiRoot = UiRootSingleton.Instance;
        if (uiRoot == null || interactPanel == null)
            return;

        Node instance = interactPanel.Instantiate();
        if (instance is not Control control)
        {
            instance.QueueFree();
            return;
        }

        _interactPanelReference = control;
        uiRoot.AddChild(control);
        UpdateInteractPanelPosition();
    }

    private void DespawnInteractPanel()
    {
        _interactPanelReference?.QueueFree();
        _interactPanelReference = null;
    }

    private void UpdateInteractPanelPosition()
    {
        if (_interactPanelReference == null)
            return;

        Camera3D? camera = CameraSpringArmSingleton.Instance?.camera ?? GetViewport().GetCamera3D();
        UiRootSingleton? uiRoot = UiRootSingleton.Instance;

        if (camera == null || uiRoot == null)
        {
            _interactPanelReference.Visible = false;
            return;
        }

        Vector3 worldPosition = GlobalPosition + interactPanelWorldOffset;
        if (camera.IsPositionBehind(worldPosition))
        {
            _interactPanelReference.Visible = false;
            return;
        }

        Vector2 cameraViewportSize = camera.GetViewport().GetVisibleRect().Size;
        Vector2 uiViewportSize = uiRoot.GetViewport().GetVisibleRect().Size;

        if (
            cameraViewportSize.X <= 0.0f
            || cameraViewportSize.Y <= 0.0f
            || uiViewportSize.X <= 0.0f
            || uiViewportSize.Y <= 0.0f
        )
        {
            _interactPanelReference.Visible = false;
            return;
        }

        Vector2 cameraViewportPosition = camera.UnprojectPosition(worldPosition);
        Vector2 uiPosition = new(
            cameraViewportPosition.X * uiViewportSize.X / cameraViewportSize.X,
            cameraViewportPosition.Y * uiViewportSize.Y / cameraViewportSize.Y
        );

        Vector2 panelPosition = uiPosition - (_interactPanelReference.Size * 0.5f);

        _interactPanelReference.Visible = true;
        _interactPanelReference.GlobalPosition = new Vector2(
            Mathf.Round(panelPosition.X),
            Mathf.Round(panelPosition.Y)
        );
    }

    private static void SetNodeActive(Node3D? node, bool active)
    {
        if (node == null)
            return;

        node.Visible = active;
        node.ProcessMode = active ? ProcessModeEnum.Inherit : ProcessModeEnum.Disabled;
    }
}
