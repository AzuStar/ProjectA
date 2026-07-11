using Godot;
using ProjectA.Game.Singletons;

namespace ProjectA.Game.Player;

public partial class PlayerCharacterController : CharacterBody3D
{
    private const string IdleAnimation = "KayKitAnim/Idle_A";
    private const string WalkingAnimation = "KayKitAnimMovement/Walking_B";

    [Export]
    public float Speed = 4.0f;

    [Export]
    public double AnimationBlendSeconds = 0.15;

    [Export]
    public bool RotateTowardInput = true;

    [Export]
    public bool DriveAnimationPlayer = true;

    [Export]
    public bool DriveCameraSmoothingTarget = true;

    public bool acceptInput = true;

    private AnimationPlayer? _animationPlayer;
    private string _currentAnimation = string.Empty;

    public override void _Ready()
    {
        _animationPlayer = DriveAnimationPlayer ? GetNodeOrNull<AnimationPlayer>("AnimationPlayer") : null;
        PlayAnimation(IdleAnimation);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!acceptInput)
        {
            PlayAnimation(IdleAnimation);
            return;
        }

        Vector3 direction = GetMovementDirection();

        if (direction == Vector3.Zero)
        {
            UpdateCameraSmoothingTarget();
            PlayAnimation(IdleAnimation);
            return;
        }

        MoveOnNavmesh(direction, (float)delta);
        UpdateCameraSmoothingTarget();

        if (RotateTowardInput)
        {
            LookAt(GlobalPosition + direction, Vector3.Up);
        }

        PlayAnimation(WalkingAnimation);
    }

    private void MoveOnNavmesh(Vector3 direction, float delta)
    {
        Vector3 desiredPosition = GlobalPosition + (direction * Speed * delta);
        GlobalPosition = NavigationServer3D.MapGetClosestPoint(GetWorld3D().NavigationMap, desiredPosition);
    }

    private void UpdateCameraSmoothingTarget()
    {
        if (!DriveCameraSmoothingTarget)
            return;

        CameraSpringArmSingleton? springArm = CameraSpringArmSingleton.Instance;
        if (springArm != null)
            springArm.smoothingTargetPosition = GlobalPosition;
    }

    private Vector3 GetMovementDirection()
    {
        Vector3 inputDirection = GetInputDirection();
        if (inputDirection == Vector3.Zero)
            return Vector3.Zero;

        CameraSpringArmSingleton? springArm = CameraSpringArmSingleton.Instance;
        if (springArm == null)
            return inputDirection;

        return inputDirection.Rotated(Vector3.Up, springArm.MovementYawRadians).Normalized();
    }

    private static Vector3 GetInputDirection()
    {
        Vector3 direction = Vector3.Zero;

        if (Input.IsKeyPressed(Key.W))
        {
            direction.Z -= 1.0f;
        }

        if (Input.IsKeyPressed(Key.S))
        {
            direction.Z += 1.0f;
        }

        if (Input.IsKeyPressed(Key.A))
        {
            direction.X -= 1.0f;
        }

        if (Input.IsKeyPressed(Key.D))
        {
            direction.X += 1.0f;
        }

        return direction == Vector3.Zero ? Vector3.Zero : direction.Normalized();
    }

    private void PlayAnimation(string animationName)
    {
        if (!DriveAnimationPlayer || _animationPlayer == null || _currentAnimation == animationName)
        {
            return;
        }

        _animationPlayer.Play(animationName, AnimationBlendSeconds);
        _currentAnimation = animationName;
    }
}
