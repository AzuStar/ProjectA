using System;
using Godot;

namespace ProjectA.Game;

[Tool]
[GlobalClass]
public partial class RaycastArc3D : Node3D
{
    const string RaycastNamePrefix = "Raycast";

    [Export(PropertyHint.Range, "0,128,1,or_greater")]
    public int raycastCount = 6;

    [Export]
    public float arcAngle = 45.0f;

    [Export]
    public float raycastLength = 10.0f;

    [Export]
    public bool raycastsEnabled = true;

    [Export]
    public Color debugShapeCustomColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);

    [Export(PropertyHint.Range, "1,16,1,or_greater")]
    public int debugShapeThickness = 1;

    int lastRaycastCount = -1;
    float lastArcAngle;
    float lastRaycastLength;
    bool lastRaycastsEnabled;
    Color lastDebugShapeCustomColor;
    int lastDebugShapeThickness;
    RayCast3D[] raycasts = Array.Empty<RayCast3D>();

    public override void _Ready()
    {
        if (Engine.IsEditorHint())
            RebuildRaycasts();
        else
            CacheRaycasts();
    }

    public override void _Process(double delta)
    {
        if (!Engine.IsEditorHint())
            return;

        if (SettingsChanged())
            RebuildRaycasts();
    }

    void RebuildRaycasts()
    {
        RemoveGeneratedRaycasts();

        for (int i = 0; i < raycastCount; i++)
        {
            RayCast3D raycast = new RayCast3D();
            raycast.Name = $"{RaycastNamePrefix}{i:D2}";
            raycast.Enabled = raycastsEnabled;
            raycast.TargetPosition = new Vector3(0.0f, 0.0f, -raycastLength);
            raycast.DebugShapeCustomColor = debugShapeCustomColor;
            raycast.DebugShapeThickness = debugShapeThickness;
            raycast.Rotation = new Vector3(0.0f, Mathf.DegToRad(GetRaycastAngle(i)), 0.0f);

            AddChild(raycast);

            if (Owner != null)
                raycast.Owner = Owner;
        }

        SaveSettings();
        CacheRaycasts();
    }

    void RemoveGeneratedRaycasts()
    {
        for (int i = GetChildCount() - 1; i >= 0; i--)
        {
            Node child = GetChild(i);

            if (!child.Name.ToString().StartsWith(RaycastNamePrefix, StringComparison.Ordinal))
                continue;

            RemoveChild(child);
            child.Free();
        }
    }

    float GetRaycastAngle(int index)
    {
        if (raycastCount <= 1)
            return 0.0f;

        float halfAngle = arcAngle * 0.5f;
        float step = arcAngle / (raycastCount - 1);
        return -halfAngle + step * index;
    }

    bool SettingsChanged()
    {
        return raycastCount != lastRaycastCount
            || arcAngle != lastArcAngle
            || raycastLength != lastRaycastLength
            || raycastsEnabled != lastRaycastsEnabled
            || debugShapeCustomColor != lastDebugShapeCustomColor
            || debugShapeThickness != lastDebugShapeThickness;
    }

    void SaveSettings()
    {
        lastRaycastCount = raycastCount;
        lastArcAngle = arcAngle;
        lastRaycastLength = raycastLength;
        lastRaycastsEnabled = raycastsEnabled;
        lastDebugShapeCustomColor = debugShapeCustomColor;
        lastDebugShapeThickness = debugShapeThickness;
    }

    void CacheRaycasts()
    {
        raycasts = new RayCast3D[raycastCount];

        for (int i = 0; i < raycastCount; i++)
            raycasts[i] = GetNode<RayCast3D>($"{RaycastNamePrefix}{i:D2}");
    }

    public int RaycastCount => raycasts.Length;

    public RayCast3D GetRaycast(int index) => raycasts[index];
}
