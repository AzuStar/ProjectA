using Godot;

namespace ProjectA.Game;

[Tool]
public partial class CameraSpringArmSingleton : Node3D
{
    public static CameraSpringArmSingleton? Instance { get; private set; }

    [Export]
    public Camera3D? camera;

    [Export]
    public Vector3 cameraRotationDegrees = new Vector3(-47.5f, 0.0f, 0.0f);

    [Export(PropertyHint.Range, "0.0,100.0,0.01,or_greater")]
    public float cameraDistance = 7;

    [Export]
    public Vector2 cameraOffset = new(0, 0);

    [Export]
    public Vector3 smoothingTargetPosition = Vector3.Zero;

    [Export]
    public bool smoothPosition = true;

    [Export(PropertyHint.Range, "0.01,60.0,0.01,or_greater")]
    public float positionSmoothingSpeed = 12.0f;

    [Export(PropertyHint.Range, "0.0,100.0,0.1,or_greater")]
    public float positionSnapDistance = 25.0f;

    public float MovementYawRadians => GlobalRotation.Y + Mathf.DegToRad(cameraRotationDegrees.Y);

    public override void _EnterTree()
    {
        Instance = this;
    }

    public override void _ExitTree()
    {
        if (Instance == this)
            Instance = null;
    }

    public override void _Ready()
    {
        if (smoothingTargetPosition == Vector3.Zero)
            smoothingTargetPosition = GlobalPosition;

        ApplyCameraSettings();
    }

    public override void _Process(double delta)
    {
        if (!Engine.IsEditorHint())
            SmoothToTarget((float)delta);

        ApplyCameraSettings();
    }

    public void SnapToSmoothingTarget()
    {
        GlobalPosition = smoothingTargetPosition;
    }

    private void SmoothToTarget(float delta)
    {
        if (!smoothPosition || positionSmoothingSpeed <= 0.0f)
        {
            GlobalPosition = smoothingTargetPosition;
            return;
        }

        if (
            positionSnapDistance > 0.0f
            && GlobalPosition.DistanceSquaredTo(smoothingTargetPosition) > positionSnapDistance * positionSnapDistance
        )
        {
            SnapToSmoothingTarget();
            return;
        }

        float weight = 1.0f - Mathf.Exp(-positionSmoothingSpeed * delta);
        GlobalPosition = GlobalPosition.Lerp(smoothingTargetPosition, weight);
    }

    private void ApplyCameraSettings()
    {
        GlobalRotationDegrees = Vector3.Zero;

        if (camera == null)
            return;

        camera.RotationDegrees = cameraRotationDegrees;
        Basis cameraBasis = camera.Transform.Basis.Orthonormalized();
        camera.Position =
            (cameraBasis.X * cameraOffset.X) + (cameraBasis.Y * cameraOffset.Y) + (cameraBasis.Z * cameraDistance);
    }
}
