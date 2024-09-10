using Godot;
using System.Collections.Generic;
using Visualize;
using Visualize.Utils;

namespace Visualize.Example;

[Visualize(nameof(Position), nameof(Offset), nameof(Rotation))]
public partial class VisualizeExample : Sprite2D
{
	[Visualize] Vector2I position;
    [Visualize] float rotation;
    [Visualize] Color color = Colors.White;
    [Visualize] float skew;
    [Visualize] Vector2 offset;

    private readonly VisualLogger logger = new();

    public override void _PhysicsProcess(double delta)
    {
        Position = position;
        Rotation = rotation;
        Modulate = color;
        Skew = skew;
        Offset = offset;
    }

    [Visualize]
    public void PrintDictionary(Dictionary<int, Vector4> dictionary)
    {
        if (dictionary == null || dictionary.Count == 0)
        {
            logger.Log("Method dictionary param has no elements", this);
        }
        else
        {
            string logMessage = "[\n";

            foreach (KeyValuePair<int, Vector4> kvp in dictionary)
            {
                logMessage += $"    {{ {kvp.Key}, {kvp.Value} }},\n";
            }

            logMessage = logMessage.TrimEnd('\n', ',') + "\n]";

            logger.Log(logMessage, this);
        }
    }

    [Visualize]
    public void PrintEnum(SomeEnum someEnum)
    {
        logger.Log(someEnum, this);
    }

    public enum SomeEnum
    {
        One,
        Two,
        Three
    }
}
