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
    RaycastArc3D[] viewRaycastArcs;

    public override void _Ready()
    {
        PlayAnimation(IdleAnimation);
        chasingState.SetIdleTarget(GlobalPosition);
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
        for (int i = 0; i < viewRaycastArcs.Length; i++)
        {
            RaycastArc3D viewRaycastArc = viewRaycastArcs[i];

            for (int j = 0; j < viewRaycastArc.RaycastCount; j++)
            {
                RayCast3D raycast = viewRaycastArc.GetRaycast(j);

                if (!raycast.IsColliding())
                    continue;

                Node3D collider = (Node3D)raycast.GetCollider();

                if (collider.IsInGroup(GroupsTable.PLAYER))
                {
                    SetLastSeenPlayerPosition(collider);
                    return;
                }
            }
        }
    }

    void SetLastSeenPlayerPosition(Node3D player)
    {
        if (stateMachine.currentState is not PatrollingState && stateMachine.currentState is not SearchState)
            return;

        chasingState.SetGoal(player.GlobalPosition);
        patrollingState.ChaseTime();
        searchState.TargetFound();
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
