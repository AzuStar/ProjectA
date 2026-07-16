using Godot;

namespace ProjectA.Game.UI;

using Godot;

[GlobalClass]
public partial class UiNode3DPositionSync : Node
{
    [Export]
    public Node3D syncFrom;

    [Export]
    public Vector3 worldOffset;

    [Export]
    public Vector2 canvasOffset;

    [Export]
    public Control syncTo;

    [Export]
    public bool centerHorizontally = true;

    [Export]
    public bool placeAbovePosition = true;

    [Export]
    public bool hideWhenOffscreen = true;

    public override void _Process(double delta)
    {
        // bug! when instance becomes invalid godot doesn't actually set it to null, only the pointer is flushed
        if (!IsInstanceValid(syncFrom) || !IsInstanceValid(syncTo))
        {
            return;
        }

        SubViewport gameViewport = Bootstrap.GetGameSubViewport();
        SubViewport uiViewport = Bootstrap.GetUiSubViewport();
        Camera3D camera = gameViewport.GetCamera3D();

        if (camera == null)
        {
            syncTo.Visible = false;
            return;
        }

        Vector3 worldPosition = syncFrom.GlobalPosition + worldOffset;

        if (camera.IsPositionBehind(worldPosition))
        {
            syncTo.Visible = false;
            return;
        }

        Vector2 gameViewportSize = gameViewport.GetVisibleRect().Size;
        Vector2 uiViewportSize = uiViewport.GetVisibleRect().Size;

        if (gameViewportSize.X <= 0.0f || gameViewportSize.Y <= 0.0f)
        {
            syncTo.Visible = false;
            return;
        }

        Vector2 projectedPosition = camera.UnprojectPosition(worldPosition);

        Vector2 uiPosition = new(
            projectedPosition.X * uiViewportSize.X / gameViewportSize.X,
            projectedPosition.Y * uiViewportSize.Y / gameViewportSize.Y
        );

        if (hideWhenOffscreen)
        {
            bool isOffscreen =
                uiPosition.X < 0.0f
                || uiPosition.Y < 0.0f
                || uiPosition.X > uiViewportSize.X
                || uiPosition.Y > uiViewportSize.Y;

            if (isOffscreen)
            {
                syncTo.Visible = false;
                return;
            }
        }

        Vector2 alignmentOffset = Vector2.Zero;

        if (centerHorizontally)
            alignmentOffset.X -= syncTo.Size.X * 0.5f;

        if (placeAbovePosition)
            alignmentOffset.Y -= syncTo.Size.Y;

        syncTo.Visible = true;
        syncTo.GlobalPosition = (uiPosition + canvasOffset + alignmentOffset).Round();
    }
}
