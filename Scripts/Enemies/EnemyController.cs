using Godot;
using ProjectA.Game;
using ProjectA.Game.Tables;

namespace ProjectA.Game.Enemies;
public partial class EnemyController : CharacterBody3D, IEnemyAnimationController
{
    public const string IdleAnimation = "KayKitAnim/Idle_A";
    public const string DeathAnimation = "KayKitAnim/Death_A";
    public const string WalkingAnimation = "KayKitAnimMovement/Walking_B";

    [Export]
    AnimationPlayer animationPlayer;

    [ExportGroup("Patrol")]
    [Export]
    Path3D patrolPath;

    [Export]
    float speed = 3.0f;

    [Export]
    float searchTurnDuration = 1.0f;

    [ExportGroup("States")]
    [Export]
    StateMachine stateMachine;

    [Export]
    PatrollingState patrollingState;

    [Export]
    ChasingState chasingState;

    [Export]
    SearchState searchState;

    [ExportGroup("View")]
    [Export]
    RaycastArc3D viewRaycastArc;

    public override void _Ready()
    {
        PlayAnimation(IdleAnimation);
        SetupStates();
    }

    public override void _PhysicsProcess(double delta)
    {
        ScanForPlayer();
    }

    void SetupStates()
    {
        patrollingState.SetPatrolPath(patrolPath);
        patrollingState.SetCharacterSpeed(speed);
        patrollingState.SetCharacter(this);
        patrollingState.SetAnimationController(this);

        chasingState.SetCharacterSpeed(speed);
        chasingState.SetCharacter(this);
        chasingState.SetAnimationController(this);

        searchState.SetAnimationController(this);

        stateMachine.StateMachineSetUp();
    }

    void ScanForPlayer()
    {
        for (int i = 0; i < viewRaycastArc.RaycastCount; i++)
        {
            RayCast3D raycast = viewRaycastArc.GetRaycast(i);

            if (!raycast.IsColliding())
                continue;

            Node collider = (Node)raycast.GetCollider();

            if (collider.IsInGroup(GroupsTable.WALLS))
                continue;

            if (collider.IsInGroup(GroupsTable.PLAYER_MAGE))
            {
                chasingState.SetGoal((Node3D)collider);
                patrollingState.ChaseTime();
                searchState.TargetFound();
                return;
            }
        }
    }

    void PlayAnimation(string animationName)
    {
        if (animationPlayer.CurrentAnimation == animationName)
            return;

        animationPlayer.Play(animationName);
    }

    public void PlayIdleAnimation() => PlayAnimation(IdleAnimation);

    public void PlayMovementAnimation() => PlayAnimation(WalkingAnimation);
}
