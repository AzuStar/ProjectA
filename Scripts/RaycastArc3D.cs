using System;
using Godot;

namespace ProjectA.Game;

[Tool]
[GlobalClass]
public partial class RaycastArc3D : Node3D
{
    [Export(PropertyHint.Range, "0,128,1,or_greater")]
    public int raycastCount = 6;

    [Export]
    public float arcAngle = 45.0f;

    [Export]
    public PackedScene raycastPrefab;

    int lastRaycastCount = -1;
    float lastArcAngle;
    PackedScene lastRaycastPrefab;
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
        if (raycastPrefab == null)
        {
            SaveSettings();
            return;
        }

        RemoveGeneratedRaycasts();

        for (int i = 0; i < raycastCount; i++)
        {
            RayCast3D raycast = raycastPrefab.Instantiate<RayCast3D>();
            raycast.Position = Vector3.Zero;
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

            if (child is not RayCast3D)
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
        return raycastCount != lastRaycastCount || arcAngle != lastArcAngle || raycastPrefab != lastRaycastPrefab;
    }

    void SaveSettings()
    {
        lastRaycastCount = raycastCount;
        lastArcAngle = arcAngle;
        lastRaycastPrefab = raycastPrefab;
    }

    void CacheRaycasts()
    {
        raycasts = new RayCast3D[raycastCount];

        for (int i = 0; i < raycastCount; i++)
            raycasts[i] = GetChild<RayCast3D>(i);
    }

    public int RaycastCount => raycasts.Length;

    public RayCast3D GetRaycast(int index) => raycasts[index];
}
