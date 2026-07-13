using System.Collections.Generic;
using Godot;

public partial class EnemyController : CharacterBody3D
{
    private const string IdleAnimation = "KayKitAnim/Idle_A";

    [ExportGroup("Patrol")]
    [Export]
    Node3D[] route;

    [Export]
    AnimationPlayer animationPlayer;

    [Export]
    float speed = 3.0f;

    [Export]
    float searchTime = 2.0f;

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
    RayCast3D viewRaycastTemplate;

    [Export]
    int viewRaycastCount = 6;

    [Export]
    float viewAngle = 45.0f;

    [Export]
    float viewSize = 10.0f;

    readonly List<RayCast3D> viewRaycasts = new List<RayCast3D>();

    public override void _Ready()
    {
        PlayAnimation(IdleAnimation);
        SetupStates();
        SetupViewRaycasts();
    }

    public override void _PhysicsProcess(double delta)
    {
        ScanForPlayer();
    }

    void SetupStates()
    {
        patrollingState.SetRoute(route);
        patrollingState.SetCharacterSpeed(speed);
        patrollingState.SetCharacter(this);

        searchState.SetCharacter(this);
        searchState.SetSearchTime(searchTime);
        searchState.SetTurnDurations(searchTurnDuration);

        chasingState.SetCharacterSpeed(speed);
        chasingState.SetCharacter(this);

        stateMachine.StateMachineSetUp();
    }

    void SetupViewRaycasts()
    {
        viewRaycastTemplate.TargetPosition = new Vector3(0.0f, 0.0f, -viewSize);
        viewRaycastTemplate.Enabled = true;
        viewRaycasts.Add(viewRaycastTemplate);

        for (int i = 1; i < viewRaycastCount; i++)
        {
            RayCast3D raycast = (RayCast3D)viewRaycastTemplate.Duplicate();
            viewRaycastTemplate.AddSibling(raycast);
            raycast.Transform = viewRaycastTemplate.Transform;
            raycast.Enabled = true;
            viewRaycasts.Add(raycast);
        }

        FanOutViewRaycasts();
    }

    void FanOutViewRaycasts()
    {
        float halfAngle = viewAngle * 0.5f;
        float step = viewRaycastCount <= 1 ? 0.0f : viewAngle / (viewRaycastCount - 1);

        for (int i = 0; i < viewRaycasts.Count; i++)
        {
            float offset = -halfAngle + step * i;
            RayCast3D raycast = viewRaycasts[i];
            raycast.Rotation = new Vector3(0.0f, viewRaycastTemplate.Rotation.Y + Mathf.DegToRad(offset), 0.0f);
        }
    }

    void ScanForPlayer()
    {
        for (int i = 0; i < viewRaycasts.Count; i++)
        {
            RayCast3D raycast = viewRaycasts[i];

            if (!raycast.IsColliding())
                continue;

            Node collider = (Node)raycast.GetCollider();

            if (collider.IsInGroup("Wall"))
                continue;

            if (collider.IsInGroup("Player"))
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
}
