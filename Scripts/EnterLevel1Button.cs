using Godot;
using ProjectA.Game.Singletons;

namespace ProjectA.Game;

public partial class EnterLevel1Button : Button
{
    public override void _Pressed()
    {
        GameManagerSingleton.MoveToNextLevel();
        QueueFree();
        base._Pressed();
    }
}
