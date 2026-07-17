using Godot;

public partial class RespawnTrigger : Area3D
{
	public override void _Ready()
    {
        BodyEntered += HandleBodyEntered;
    }

    private void HandleBodyEntered(Node3D body)
    {
        if (body is PushableBody pushable && pushable.RespondsToRespawnTriggers)
        {
            pushable.Respawn();
        }
    }
}
