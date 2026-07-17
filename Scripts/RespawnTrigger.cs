using Godot;

public partial class RespawnTrigger : Area3D
{
	public void SetEnabled(bool enabled)
    {
        if (enabled)
        {
            BodyEntered += HandleBodyEntered;
        }
        else
        {
            BodyEntered -= HandleBodyEntered;
        }
    }

    private void HandleBodyEntered(Node3D body)
    {
        if (body is PushableBody pushable)
        {
            pushable.Respawn();
        }
    }
}
