namespace Template;

[Tool]
public partial class Torch : Node2D
{
    [Visualize] [Export] double energy = 1;
    [Export] double flickerRange = 0.05;
    [Export] double pulseAmplitude = 0.1;
    [Export] float textureScale
    {
        get => _textureScale;
        set
        {
            if (light != null)
            {
                _textureScale = value;
                light.TextureScale = _textureScale;
            }
        }
    }

    PointLight2D light;
    float _textureScale = 1;

    public override void _Ready()
    {
        light = GetNode<PointLight2D>("PointLight2D");
    }

    public override void _PhysicsProcess(double delta)
    {
        light.Energy = (float)(energy + GD.RandRange(0, flickerRange) - 
            Mathf.Sin(Engine.GetPhysicsFrames() * 0.01) * pulseAmplitude);
    }

    public enum Test
    {
        One,
        Two,
        Three
    }

    [Visualize]
    public void TestMethod(Color color, int someInt, bool someBool, Test test)
    {
        Game.Log(color, someInt, someBool, test);
    }
}
