using Godot;

namespace ProjectA.Game;

public partial class MouseCapture : Node
{
    public override void _Input(InputEvent ev)
    {
        if (ev.IsActionPressed("drop_focus"))
        {
            SetMouseCaptured(false);
        }
    }

    public override void _Notification(int what)
    {
        switch (what)
        {
            case (int)NotificationApplicationFocusIn:
                SetMouseCaptured(true);
                break;
            case (int)NotificationApplicationFocusOut:
                SetMouseCaptured(false);
                break;
        }
    }

    private void SetMouseCaptured(bool captured)
    {
        Input.MouseMode = captured ? Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible;
    }
}