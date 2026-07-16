using Godot;

namespace ProjectA.Game.Singletons;

public partial class PlayerSingleton
{
    [Export]
    public float cameraMouseSensitivity = 0.0025f;

    // =====================================================
    [ExportGroup("Drone Leash")]
    [Export]
    public float maxDroneLeashRange = 10.0f;

    [Export]
    public Vector2 droneScreenMaterialCloseDimensions;

    [Export]
    public Vector2 droneScreenMaterialFarDimensions;

    [Export]
    public Curve droneScreenMaterialDimensionsCurve;

    // =====================================================
    [ExportGroup("Mage Config")]
    [Export]
    public float mageMovementSpeed;

    // =====================================================
    [ExportGroup("Drone Config")]
    [Export]
    public float droneManualUnsummonCooldown = 1.0f;

    [Export]
    public float droneDeathTriggerCooldown = 10.0f;

    [Export]
    public float droneMovementMaxVelocity = 4;

    [Export]
    public float droneAcceleration = 4;
}
