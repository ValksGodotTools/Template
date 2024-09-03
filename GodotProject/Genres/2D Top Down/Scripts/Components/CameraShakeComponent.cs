namespace Template;

[GlobalClass, Icon("res://Template/Sprites/Icons/Gear/gear.svg")]
public partial class CameraShakeComponent : Node
{
    CameraShakeConfig config;
    Camera2D camera;

    double remainingTime;
    double freqCounter;

	public override void _Ready()
	{
        camera = GetParent<Camera2D>();
        config = new();
        SetPhysicsProcess(false);
	}

	public override void _PhysicsProcess(double delta)
	{
        // Constantly subtract from remaining time
        remainingTime -= delta;
        freqCounter += delta;

        // Update offset at frequency
        if (freqCounter >= config.Frequency)
        {
            freqCounter = 0;
            camera.Offset = new Vector2((GD.Randf() - 0.5f), (GD.Randf() - 0.5f)) * config.Amplitude;
        }

        // Stop shaking and reset camera offset when remaining time reaches zero
        if (remainingTime <= 0)
        {
            camera.Offset = Vector2.Zero;
            SetPhysicsProcess(false);
        }
    }

    public void Start(CameraShakeConfig newConfig)
    {
        // Duration is accumulative
        remainingTime += newConfig.Duration;

        // Only set new amplitude if it's greater than current amplitude
        if (newConfig.Amplitude > config.Amplitude)
            config.Amplitude = newConfig.Amplitude;

        // Update frequency
        config.Frequency = newConfig.Frequency;

        // Start shaking
        SetPhysicsProcess(true);
    }
}

public class CameraShakeConfig
{
    public Vector2 Amplitude { get; set; }
    public double Duration { get; set; } = 1.0;
    public double Frequency { get; set; }
}
