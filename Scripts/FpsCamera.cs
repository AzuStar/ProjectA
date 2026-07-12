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
}
