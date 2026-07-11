using Godot;

namespace ProjectA.Game;

public partial class UiRootSingleton : Control
{
    public static UiRootSingleton? Instance { get; private set; }

    [Export]
    public RichTextLabel firstLabel;

    public override void _EnterTree()
    {
        Instance = this;
    }

    public override void _ExitTree()
    {
        if (Instance == this)
            Instance = null;
    }
}
