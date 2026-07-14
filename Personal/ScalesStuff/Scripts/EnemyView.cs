using Godot;
using System;

public partial class EnemyView : RayCast3D
{
	[Signal] delegate void PlayerFoundEventHandler();
	[Signal] delegate void PlayerPositionEventHandler(Node3D player);

	public override void _Process(double delta)
	{
		if (!IsColliding()) return;

		Node collider = GetCollider() as Node;

		if (collider.IsInGroup("Wall"))
		{
			GD.Print("Wall");
			return;
		}
		if (collider.IsInGroup("Player"))
		{
			EmitSignal(SignalName.PlayerFound);
			EmitSignal(SignalName.PlayerPosition,(collider as Node3D));
		}
	}
}
