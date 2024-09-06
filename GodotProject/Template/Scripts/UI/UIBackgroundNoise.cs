using Godot;

namespace Template;

public partial class UIBackgroundNoise : Sprite2D
{
    const int SPEED = 20;
    const int AMPLITUDE_X = 200;
    const int AMPLITUDE_Y = 100;

    FastNoiseLite fnlAngle1, fnlAngle2;
    static float timeElpased;
    Vector2 defaultPosition;

    public override void _Ready()
    {
        fnlAngle1 = InitNoise(seed: 29048);
        fnlAngle2 = InitNoise(seed: 50000);
        defaultPosition = Position;

        ResizeBackground();
        GetTree().Root.SizeChanged += ResizeBackground;
    }

    public override void _Process(double delta)
    {
        timeElpased += (float)delta * SPEED;

        float angle1 = fnlAngle1.GetNoise1D(timeElpased);
        float angle2 = fnlAngle2.GetNoise1D(timeElpased);

        Vector2 offset = new(
            Mathf.Cos(angle1) * AMPLITUDE_X - AMPLITUDE_X, 
            Mathf.Cos(angle2) * AMPLITUDE_Y - AMPLITUDE_Y);

        Position = defaultPosition + offset;
    }

    void ResizeBackground()
    {
        Vector2I winSize = DisplayServer.WindowGetSize();
        Vector2 texSize = Texture.GetSize();

        // This is the zoom that is added so the picture can move around
        // This calculation was eye-balled, there has to be a 100% accurate way
        // of figuring how much zoom is needed without the noise going outside
        // the pictures bounds
        float zoom = (float)AMPLITUDE_X / winSize.X / 8;

        Position = winSize / 2;
        Scale = Vector2.One * ((winSize / texSize).X + zoom);

        defaultPosition = Position;
    }

    FastNoiseLite InitNoise(int seed = 0)
    {
        return new FastNoiseLite
        {
            Seed = seed,
            NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin,
            FractalType = FastNoiseLite.FractalTypeEnum.None,
            Frequency = 0.01f
        };
    }
}

