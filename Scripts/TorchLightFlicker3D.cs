using Godot;

namespace ProjectA.Game;

public partial class TorchLightFlicker3D : OmniLight3D
{
    [Export]
    public float BaseEnergy = 2.8f;

    [Export]
    public float EnergyFlickerAmount = 0.45f;

    [Export]
    public float BaseRange = 5.5f;

    [Export]
    public float RangeFlickerAmount = 0.35f;

    [Export]
    public float PositionJitterAmount = 0.035f;

    [Export]
    public float PositionJitterSpeed = 5.0f;

    [Export]
    public float SlowFlickerSpeed = 1.7f;

    [Export]
    public float FastFlickerSpeed = 11.0f;

    [Export]
    public Color WarmColor = new Color(1.0f, 0.48f, 0.16f);

    [Export]
    public Color HotColor = new Color(1.0f, 0.78f, 0.38f);

    [Export]
    public float Smoothing = 14.0f;

    private Vector3 _basePosition;
    private float _time;

    private readonly FastNoiseLite _slowNoise = new();
    private readonly FastNoiseLite _fastNoise = new();
    private readonly FastNoiseLite _posNoise = new();

    private readonly RandomNumberGenerator _rng = new();

    private float _burst;
    private float _burstVelocity;

    public override void _Ready()
    {
        _basePosition = Position;

        _rng.Randomize();

        SetupNoise(_slowNoise, _rng.Randi(), 0.8f);
        SetupNoise(_fastNoise, _rng.Randi(), 3.0f);
        SetupNoise(_posNoise, _rng.Randi(), 1.4f);

        LightColor = WarmColor;
        LightEnergy = BaseEnergy;
        OmniRange = BaseRange;
    }

    public override void _Process(double delta)
    {
        float dt = (float)delta;
        _time += dt;

        float slow = RemapNoise(_slowNoise.GetNoise1D(_time * SlowFlickerSpeed));

        float fast = RemapNoise(_fastNoise.GetNoise1D(_time * FastFlickerSpeed));

        if (_rng.Randf() < dt * 1.4f)
            _burstVelocity += _rng.RandfRange(-0.55f, 0.85f);

        _burst += _burstVelocity * dt;
        _burstVelocity = Mathf.Lerp(_burstVelocity, -_burst * 8.0f, dt * 8.0f);
        _burst *= Mathf.Exp(-dt * 7.5f);
        _burst = Mathf.Clamp(_burst, -0.45f, 0.65f);

        float flicker = 0.55f * slow + 0.30f * fast + 0.15f * _burst;

        flicker = Mathf.Clamp(flicker, 0.0f, 1.0f);

        float targetEnergy = BaseEnergy * (1.0f + (flicker - 0.5f) * 2.0f * EnergyFlickerAmount);
        float targetRange = BaseRange + (flicker - 0.5f) * 2.0f * RangeFlickerAmount;

        Color targetColor = WarmColor.Lerp(HotColor, Mathf.Clamp(flicker * 1.2f, 0.0f, 1.0f));

        float lerp = 1.0f - Mathf.Exp(-Smoothing * dt);

        LightEnergy = Mathf.Lerp(LightEnergy, targetEnergy, lerp);
        OmniRange = Mathf.Lerp(OmniRange, targetRange, lerp);
        LightColor = LightColor.Lerp(targetColor, lerp);

        float px = _posNoise.GetNoise2D(_time * PositionJitterSpeed, 11.0f);
        float py = _posNoise.GetNoise2D(_time * PositionJitterSpeed, 29.0f);
        float pz = _posNoise.GetNoise2D(_time * PositionJitterSpeed, 47.0f);

        Vector3 jitter = new Vector3(px, py * 0.7f, pz) * PositionJitterAmount;
        Position = _basePosition + jitter;
    }

    private static void SetupNoise(FastNoiseLite noise, uint seed, float frequency)
    {
        noise.Seed = (int)seed;
        noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
        noise.Frequency = frequency;
    }

    private static float RemapNoise(float value)
    {
        return Mathf.Clamp((value + 1.0f) * 0.5f, 0.0f, 1.0f);
    }
}
