using Godot;
using ProjectA.Game.Levels;
using ProjectA.Game.Player;

public partial class KillTrigger : Area3D
{
	public override void _Ready()
	{
		BodyEntered += HandleBodyEntered;
	}

	private void HandleBodyEntered(Node3D body)
	{
		if (!LevelInstance.Current.IsPlayerDuoPrepared || body is not PlayerCharacterController)
		{
			return;
		}

		LevelInstance.Current.CurrentGameState = LevelInstance.GameState.Dead;
	}
}
