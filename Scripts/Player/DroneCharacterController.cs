using Godot;
using ProjectA.Game;

namespace ProjectA.Game.Player;

public partial class DroneCharacterController : CharacterBody3D
{
    [Export]
    public float MaximumSpeed = 4.0f;

    [Export]
    public float Acceleration = 8.0f;

    [Export]
    public float BounceRestitution = 0.5f;

    [Export]
    public bool RotateTowardInput = true;

    [Export]
    public float SpawnHeightOffset = 2.5f;

    [Export]
    public bool DriveCameraSmoothingTarget = true;

    [Export]
    public FpsCamera fpsCamera;

    public bool acceptInput;
    private uint _enabledCollisionLayer;
    private uint _enabledCollisionMask;

    public override void _Ready()
    {
        _enabledCollisionLayer = CollisionLayer;
        _enabledCollisionMask = CollisionMask;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!acceptInput)
        {
            return;
        }

        Vector3 direction = GetMovementDirection();
        Vector3 desiredVelocity = direction * MaximumSpeed;
        Velocity = Velocity.MoveToward(desiredVelocity, Acceleration * (float)delta);
        MoveAndSlide();

        bool bounced = false;
        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            KinematicCollision3D slideCollision = GetSlideCollision(i);
            if (slideCollision != null)
            {
                bounced = true;
                Vector3 normal = slideCollision.GetNormal();
                Velocity = Velocity.Bounce(normal);
            }
        }
        if (bounced)
        {
            Velocity *= BounceRestitution;
        }
    }

    private Vector3 GetMovementDirection()
    {
        Vector3 inputDirection = GetInputDirection();
        if (inputDirection == Vector3.Zero)
            return Vector3.Zero;
        
        Basis cameraBasis = fpsCamera.fpsCamera.GlobalBasis;
        return cameraBasis * inputDirection;
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

        if (Input.IsKeyPressed(Key.Ctrl))
        {
            direction.Y -= 1.0f;
        }

        if (Input.IsKeyPressed(Key.Space))
        {
            direction.Y += 1.0f;
        }

        return direction.LengthSquared() > 0.0f ? direction.Normalized() : Vector3.Zero;
    }

    public void EnableDrone(Vector3 basePosition)
    {
        GlobalPosition = basePosition + (Vector3.Up * SpawnHeightOffset);
        Visible = true;
        acceptInput = true;
        ProcessMode = ProcessModeEnum.Inherit;
        CollisionLayer = _enabledCollisionLayer;
        CollisionMask = _enabledCollisionMask;
        fpsCamera.SetActive(true);
    }

    public void DisableDrone()
    {
        acceptInput = false;
        Visible = false;
        ProcessMode = ProcessModeEnum.Disabled;
        CollisionLayer = 0;
        CollisionMask = 0;
        fpsCamera.SetActive(false);
    }
}
