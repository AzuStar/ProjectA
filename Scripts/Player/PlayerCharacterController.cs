using Godot;
using ProjectA.Game;

namespace ProjectA.Game.Player;

public partial class PlayerCharacterController : CharacterBody3D
{
    private const string IdleAnimation = "KayKitAnim/Idle_A";
    private const string WalkingAnimation = "KayKitAnimMovement/Walking_B";

    [Export]
    public float Speed = 4.0f;

    [Export]
    public float JumpVelocity = 6.0f;

    [Export]
    public float Gravity = 24.0f;

    [Export]
    public double AnimationBlendSeconds = 0.15;

    [Export]
    public bool RotateTowardInput = true;

    [Export]
    public bool DriveAnimationPlayer = true;

    [Export]
    public bool DriveCameraSmoothingTarget = true;

    [Export]
    public Node3D visualRoot;

    [Export]
    private AnimationPlayer animationPlayer;

    [Export]
    public FpsCamera fpsCamera;

    public bool acceptInput = true;
    private string _currentAnimation = string.Empty;
    private bool _jumpWasPressed;

    public override void _Ready()
    {
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

        Move(direction, delta);
        PlayAnimation(direction == Vector3.Zero ? IdleAnimation : WalkingAnimation);
    }

    private void Move(Vector3 direction, double delta)
    {
        Vector3 velocity = Velocity;

        velocity.X = direction.X * Speed;
        velocity.Z = direction.Z * Speed;

        bool jumpPressed = Input.IsKeyPressed(Key.Space);

        if (IsOnFloor())
        {
            if (jumpPressed && !_jumpWasPressed)
                velocity.Y = JumpVelocity;
        }
        else
        {
            velocity.Y -= Gravity * (float)delta;
        }

        _jumpWasPressed = jumpPressed;
        Velocity = velocity;
        MoveAndSlide();
    }

    private Vector3 GetMovementDirection()
    {
        Vector3 inputDirection = GetInputDirection();
        if (inputDirection == Vector3.Zero)
            return Vector3.Zero;

        Basis cameraBasis = fpsCamera.fpsCamera.GlobalTransform.Basis;
        Vector3 forward = -cameraBasis.Z;
        Vector3 right = cameraBasis.X;

        forward.Y = 0.0f;
        right.Y = 0.0f;

        return (right.Normalized() * inputDirection.X - forward.Normalized() * inputDirection.Z).Normalized();
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
        if (!DriveAnimationPlayer || _currentAnimation == animationName)
        {
            return;
        }

        animationPlayer.Play(animationName, AnimationBlendSeconds);
        _currentAnimation = animationName;
    }

    public void EnablePlayer()
    {
        acceptInput = true;
        ProcessMode = ProcessModeEnum.Inherit;
        visualRoot.Visible = false;
        fpsCamera.SetActive(true);
    }

    public void DisablePlayerForDrone()
    {
        acceptInput = false;
        visualRoot.Visible = true;
        fpsCamera.SetActive(false);
        PlayAnimation(IdleAnimation);
    }
}
