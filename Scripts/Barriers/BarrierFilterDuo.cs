using Godot;
using ProjectA.Game.Player;
using ProjectA.Game.Singletons;

namespace ProjectA.Game.Barriers;

public partial class BarrierFilterDuo : Barrier
{
    [Export]
    public DuoTarget letsThrough;

    private PhysicsBody3D _currentPassBody;

    public override void _Process(double delta)
    {
        PhysicsBody3D passBody = GetPassBody();

        if (_currentPassBody == passBody)
            return;

        if (_currentPassBody != null)
            collisionObject.RemoveCollisionExceptionWith(_currentPassBody);

        _currentPassBody = passBody;
        collisionObject.AddCollisionExceptionWith(_currentPassBody);
    }

    private PhysicsBody3D GetPassBody() =>
        letsThrough == DuoTarget.PLAYER
            ? PlayerSingleton.Instance.playerDuo.player
            : PlayerSingleton.Instance.playerDuo.drone;
}
