using Godot;
using GodotUtils;

namespace Template.TopDown2D;

[Tool]
[SceneTree]
public partial class Torch : Node2D
{
    [Export] private double flickerRange = 0.05;
    [Export] private double pulseAmplitude = 0.1;
    [Export]
    private float TextureScale
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

    [Visualize] private double energy = 1;
    private PointLight2D _light;
    private float _textureScale = 1;
    private readonly VisualLogger _visualLogger = new();

    public override void _Ready()
    {
        _light = _.PointLight2D;
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
