using Godot;
using Godot.Collections;

namespace ProjectA.Game;

[Tool]
public partial class Torch : Node3D
{
    private bool litup;

    [Export]
    public bool Litup
    {
        get => litup;
        set
        {
            litup = value;
            ApplyLitup();
        }
    }

    [Export]
    public Array<OmniLight3D> lights = new();

    [Export]
    public Array<GpuParticles3D> particles = new();

    public override void _Ready()
    {
        ApplyLitup();
    }

    private void ApplyLitup()
    {
        foreach (OmniLight3D light in lights)
            light.Visible = litup;

        foreach (GpuParticles3D particle in particles)
            particle.Emitting = litup;
    }
}
