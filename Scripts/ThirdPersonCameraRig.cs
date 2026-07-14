using Godot;
using System;

public partial class ThirdPersonCameraRig : Node3D
{
	[Export] private Node3D _yawPivot;
	[Export] private Node3D _pitchPivot;
	[Export] private Camera3D _camera;
	[Export] private ShapeCast3D _raycaster;

	[Export] private float _minPitchDegrees;
	[Export] private float _maxPitchDegrees;

	public void ApplyMouseLook(Vector2 relative, float mouseSensitivity)
	{
		_yawPivot.RotateY(-relative.X * mouseSensitivity);
		_pitchPivot.RotateX(-relative.Y * mouseSensitivity);
		Vector3 pitchPivotDegrees = _pitchPivot.RotationDegrees;
		pitchPivotDegrees.X += -relative.Y * mouseSensitivity;
		pitchPivotDegrees.X = Mathf.Clamp(_pitchPivot.RotationDegrees.X, _minPitchDegrees, _maxPitchDegrees);
		_pitchPivot.RotationDegrees = pitchPivotDegrees;
	}

	public override void _Process(double delta)
	{
		RaycastCamera();
	}

	private void RaycastCamera()
	{
		Vector3 camPos;
		Vector3 globalRaycastTarget = _raycaster.GlobalBasis * _raycaster.TargetPosition;
		if (_raycaster.IsColliding())
		{
			Vector3 collisionPoint = _raycaster.GetCollisionPoint(0);
			Vector3 collisionNormal = _raycaster.GetCollisionNormal(0);
			SphereShape3D rayShape = (SphereShape3D)_raycaster.Shape;
			camPos = collisionPoint + (collisionNormal * rayShape.Radius);
		}
		else
		{
			camPos = _raycaster.GlobalPosition + globalRaycastTarget;
		}
		_camera.GlobalPosition = camPos;
		_camera.LookAt(_raycaster.GlobalPosition);
	}

	public void SetActive(bool active)
    {
        Visible = active;
        _camera.Current = active;
    }

    public Basis GetCameraBasis()
    {
	    // Yaw basis so movement is still in XZ while looking up or down.
	    return _yawPivot.GlobalBasis;
    }
}
