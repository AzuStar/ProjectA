using Godot;
using ProjectA.Game;
using ProjectA.Game.Singletons;

namespace ProjectA.Game.Player;

public partial class DroneCharacterController : CharacterBody3D
{
    [Export]
    public float MaximumSpeed = 4.0f;

    [Export]
    public float Acceleration = 8.0f;

    [Export]
    public float WallBounceRestitution = 0.5f;

    [Export]
    public float LeashSphereBounceRestitution = 0.1f;

    [Export]
    public bool RotateTowardInput = true;

    [Export]
    public float SpawnHeightOffset = 2.5f;

    [Export]
    public bool DriveCameraSmoothingTarget = true;

    [Export]
    public FpsCamera fpsCamera;

    [Export]
    public ShaderMaterial droneScreenMaterial;

    public bool acceptInput;
    private uint _enabledCollisionLayer;
    private uint _enabledCollisionMask;

    private Node3D _leashRoot;

    public override void _Ready()
    {
        _enabledCollisionLayer = CollisionLayer;
        _enabledCollisionMask = CollisionMask;
    }

    public override void _Process(double delta)
    {
        if (!acceptInput)
        {
            // Not deployed.
            return;
        }

        Vector3 leashVector = GlobalPosition - _leashRoot.GlobalPosition;
        float currentLeashLength = leashVector.Length();
        float leashNorm = Mathf.InverseLerp(0.0f,  PlayerSingleton.Instance.maxDroneLeashRange, currentLeashLength);
        float sample = PlayerSingleton.Instance.droneScreenMaterialDimensionsCurve.Sample(leashNorm);
        Vector2 desiredDimensions = PlayerSingleton.Instance.droneScreenMaterialCloseDimensions.Lerp(PlayerSingleton.Instance.droneScreenMaterialFarDimensions, sample);

        droneScreenMaterial.SetShaderParameter("target_x_pixel_count", desiredDimensions.X);
        droneScreenMaterial.SetShaderParameter("target_y_pixel_count", desiredDimensions.Y);
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

        Vector3 leashVector = GlobalPosition - _leashRoot.GlobalPosition;
        float currentLeashLengthSqr = leashVector.LengthSquared();
        float maxLeashLengthSqr = PlayerSingleton.Instance.maxDroneLeashRange * PlayerSingleton.Instance.maxDroneLeashRange;

        if (currentLeashLengthSqr > maxLeashLengthSqr)
        {
            Vector3 leashVectorNormalized = leashVector.Normalized();
            
            // Pull back.
            GlobalPosition = _leashRoot.GlobalPosition + (leashVectorNormalized * PlayerSingleton.Instance.maxDroneLeashRange);

            // Bounce off the sphere with restitution
            Velocity = Velocity.Bounce(-leashVectorNormalized) * LeashSphereBounceRestitution;
        }

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
            Velocity *= WallBounceRestitution;
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

    public void EnterThisController(Vector3 basePosition, float baseBearing, Node3D leashRoot)
    {
        _leashRoot = leashRoot;
        GlobalPosition = basePosition + (Vector3.Up * SpawnHeightOffset);
        Visible = true;
        acceptInput = true;
        ProcessMode = ProcessModeEnum.Inherit;
        CollisionLayer = _enabledCollisionLayer;
        CollisionMask = _enabledCollisionMask;
        fpsCamera.ResetPose();
        fpsCamera.SetBearing(baseBearing);
        fpsCamera.SetActive(true);

        Velocity = Vector3.Zero;

        Bootstrap.GetGameSubViewportContainer().Material = droneScreenMaterial;
    }

    public void LeaveThisController()
    {
        acceptInput = false;
        Visible = false;
        ProcessMode = ProcessModeEnum.Disabled;
        CollisionLayer = 0;
        CollisionMask = 0;
        fpsCamera.SetActive(false);

        Bootstrap.GetGameSubViewportContainer().Material = null;
    }
}
