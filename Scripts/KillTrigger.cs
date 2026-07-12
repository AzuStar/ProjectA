using Godot;
using ProjectA.Game.Levels;
using ProjectA.Game.Player;
using ProjectA.Game.Singletons;

public partial class KillTrigger : Area3D
{
    public override void _Ready()
    {
        BodyEntered += HandleBodyEntered;
    }

    private void HandleBodyEntered(Node3D body)
    {
        if (body is not PlayerCharacterController)
        {
            return;
        }

        GameManagerSingleton.currentLevelInstance.CurrentGameState = LevelInstance.GameState.Dead;
    }
}
