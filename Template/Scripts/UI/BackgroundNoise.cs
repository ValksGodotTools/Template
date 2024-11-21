using Godot;

namespace Template.UI;

public partial class BackgroundNoise : Sprite2D
{
    private const int SPEED = 20;
    private const int AMPLITUDE_X = 200;
    private const int AMPLITUDE_Y = 100;
    private FastNoiseLite _fnlAngle1, _fnlAngle2;
    private static float _timeElpased;
    private Vector2 _defaultPosition;

    public override void _Ready()
    {
        _fnlAngle1 = InitNoise(seed: 29048);
        _fnlAngle2 = InitNoise(seed: 50000);
        _defaultPosition = Position;

        ResizeBackground();
        GetTree().Root.SizeChanged += ResizeBackground;
    }

    public override void _Process(double delta)
    {
        _timeElpased += (float)delta * SPEED;

        float angle1 = _fnlAngle1.GetNoise1D(_timeElpased);
        float angle2 = _fnlAngle2.GetNoise1D(_timeElpased);

        Vector2 offset = new(
            Mathf.Cos(angle1) * AMPLITUDE_X - AMPLITUDE_X, 
            Mathf.Cos(angle2) * AMPLITUDE_Y - AMPLITUDE_Y);

        Position = _defaultPosition + offset;
    }

    private void ResizeBackground()
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

        _defaultPosition = Position;
    }

    private static FastNoiseLite InitNoise(int seed = 0)
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

