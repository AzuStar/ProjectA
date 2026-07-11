using Godot;
using Godot.Collections;

namespace ProjectA.Game;

public partial class Torch : Node3D
{
    [Export]
    public Array<OmniLight3D> lights = new();

    [Export]
    public Array<GpuParticles3D> particles = new();

    public void Lighten()
    {
        foreach (OmniLight3D light in lights)
            light.Visible = true;

        foreach (GpuParticles3D particle in particles)
            particle.Emitting = true;
    }

    public void Darken()
    {
        foreach (OmniLight3D light in lights)
            light.Visible = false;

        foreach (GpuParticles3D particle in particles)
            particle.Emitting = false;
    }
}
