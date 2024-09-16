using Godot;
using GodotUtils;

namespace Template;

[Tool]
public partial class Torch : Node2D
{
    [Export] double flickerRange = 0.05;
    [Export] double pulseAmplitude = 0.1;
    [Export] float textureScale
    {
        get => _textureScale;
        set
        {
            if (_light != null)
            {
                _textureScale = value;
                _light.TextureScale = _textureScale;
            }
        }
    }

    [Visualize] double energy = 1;
    PointLight2D _light;
    float _textureScale = 1;

    VisualLogger _visualLogger = new();

    public override void _Ready()
    {
        _light = GetNode<PointLight2D>("PointLight2D");
    }

    public override void _PhysicsProcess(double delta)
    {
        _light.Energy = (float)(energy + GD.RandRange(0, flickerRange) - 
            Mathf.Sin(Engine.GetPhysicsFrames() * 0.01) * pulseAmplitude);
    }

    [Visualize]
    public void Test(string[] myList)
    {
        _visualLogger.Log(myList.ToFormattedString(), this);
    }
}
