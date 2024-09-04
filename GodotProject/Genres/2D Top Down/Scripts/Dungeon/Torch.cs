namespace Template;

[Tool]
[DebugExports]
public partial class Torch : Node
{
    [Export] int someInteger = 1;
    [Export] [Visualize] double energy = 1;
    [Export] [Visualize] double flickerRange = 0.05;
    [Export] [Visualize] double pulseAmplitude = 0.1;
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
}