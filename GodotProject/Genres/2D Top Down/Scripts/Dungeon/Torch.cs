using Godot;
using GodotUtils;

namespace Template;

[Tool]
public partial class Torch : Node2D
{
    //[Visualize] Vector4[] vec4Array;
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

    double energy = 1;
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

    [Visualize]
    public void Test(Color[] colorArray)
    {
        //Game.Log(colorArray.Print());
    }
}
