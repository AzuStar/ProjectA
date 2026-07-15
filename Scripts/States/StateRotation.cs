using Godot;

public static class StateRotation
{
    public static void TurnTowards(Node3D node, Vector3 targetPosition, float turnSpeedDegreesPerSecond, float delta)
    {
        Vector3 direction = targetPosition - node.GlobalPosition;
        direction.Y = 0.0f;

        if (direction.LengthSquared() <= 0.0001f)
            return;

        float targetYaw = Mathf.Atan2(-direction.X, -direction.Z);
        TurnTowardsYaw(node, targetYaw, turnSpeedDegreesPerSecond, delta);
    }

    public static void TurnTowardsYaw(Node3D node, float targetYaw, float turnSpeedDegreesPerSecond, float delta)
    {
        node.Rotation = new Vector3(
            node.Rotation.X,
            Mathf.RotateToward(node.Rotation.Y, targetYaw, Mathf.DegToRad(turnSpeedDegreesPerSecond) * delta),
            node.Rotation.Z
        );
    }
}
