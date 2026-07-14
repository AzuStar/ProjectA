using Godot;

namespace ProjectA.Game;

public partial class FpsCamera : Node3D
{
    [Export]
    public Node3D applyYRotationTo;

    [Export]
    public Camera3D fpsCamera;

    [Export]
    public MeshInstance3D handMesh;

    [Export]
    public float minPitchDegrees = -85.0f;

    [Export]
    public float maxPitchDegrees = 85.0f;

    private Vector3 _startingRotation;
    private Vector3 _startingPosition;

    public override void _Ready()
    {
        _startingRotation = applyYRotationTo.Rotation; // applyYRotationTo is our parent, not our child.
        _startingPosition = Position;
    }

    public void ApplyMouseLook(Vector2 relative, float mouseSensitivity)
    {
        applyYRotationTo.RotateY(-relative.X * mouseSensitivity);

        float pitch = Mathf.Clamp(
            Rotation.X - relative.Y * mouseSensitivity,
            Mathf.DegToRad(minPitchDegrees),
            Mathf.DegToRad(maxPitchDegrees)
        );

        Rotation = new Vector3(pitch, 0.0f, 0.0f);
    }

    public void SetActive(bool active)
    {
        Visible = active;
        fpsCamera.Current = active;
    }

    public void ResetPose()
    {
        applyYRotationTo.Rotation = _startingRotation;
        Position = _startingPosition;
        Rotation = Vector3.Zero;
    }

    public void SetBearing(float bearing)
    {
        applyYRotationTo.Rotation = Vector3.Up * bearing;
    }

    public void PanOutForDeath()
    {
        SetActive(true);
        CreateTween().TweenProperty(this, "position", _startingPosition + new Vector3(0.0f, 0.85f, 2.25f), 0.7);
    }
}
