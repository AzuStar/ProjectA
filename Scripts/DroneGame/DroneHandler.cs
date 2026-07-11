using Godot;

public partial class DroneHandler : Node
{
    [Export] private DroneBody _droneBody;
    [Export] private ColorRect _viewRect;

    public void SetDroneInputActive(bool inputActive)
    {
        _droneBody.SetInputActive(inputActive);
        _viewRect.Visible = inputActive; // Slide in and out later?
    }

    public override void _Input(InputEvent ev)
    {
        _droneBody.PassInput(ev);
    }
}