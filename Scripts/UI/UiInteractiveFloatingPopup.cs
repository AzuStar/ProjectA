using Godot;

namespace ProjectA.Game.UI;

public partial class UiInteractiveFloatingPopup : Control
{
    [Export]
    public UiNode3DPositionSync positionSync;

    [Export]
    public RichTextLabel textLabel;

    public void SetText(string text)
    {
        textLabel ??= FindTextLabel(this);

        if (textLabel != null)
            textLabel.Text = text;
    }

    private static RichTextLabel FindTextLabel(Node node)
    {
        foreach (Node child in node.GetChildren())
        {
            if (child is RichTextLabel label)
                return label;

            RichTextLabel nestedLabel = FindTextLabel(child);
            if (nestedLabel != null)
                return nestedLabel;
        }

        return null;
    }
}
