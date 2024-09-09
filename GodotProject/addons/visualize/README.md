https://github.com/user-attachments/assets/a8615166-0a5c-4f8c-a144-7c0f9c5ef185

#### Example Usage
```cs
using Godot;
using System.Collections.Generic;
using Visualize;

public partial class VisualizeExample : Sprite2D
{
    [Visualize] Vector2I position;
    [Visualize] float rotation;
    [Visualize] Color color = Colors.White;
    [Visualize] float skew;

    private readonly VisualLogger logger = new();

    Vector2 initialPositionOffset;

    public override void _Ready()
    {
        initialPositionOffset = DisplayServer.WindowGetSize() / 2;
    }

    public override void _PhysicsProcess(double delta)
    {
        Position = position + initialPositionOffset;
        Rotation = rotation;
        Modulate = color;
        Skew = skew;
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
```

#### Visualizing Info at a Specific Position

You might prefer not to have the visual panel initially created at (0, 0) when visualizing members within a UI node that is always positioned at (0, 0). This can be easily adjusted by adding the `[Visualize(x, y)]` attribute at the top of the class. This attribute will set the initial position of the visual panel to the specified coordinates.

```csharp
[Visualize(200, 200)] // The visual panel will initially be positioned at (200, 200)
public partial class SomeUINode {}
```

#### Supported Members

| Member Type       | Supported  | Example Types                                 | Additional Notes                                                      |
|-------------------|------------|-----------------------------------------------|-----------------------------------------------------------------------|
| **Numericals**    | ✅         | `int`, `float`, `double`                      | All numerical types are supported                                     |
| **Enums**         | ✅         | `Direction`, `Colors`                         | All enum types are supported                                          |
| **Booleans**      | ✅         | `bool`                                        |                                                                       |
| **Strings**       | ✅         | `string`                                      |                                                                       |
| **Color**         | ✅         | `Color`                                       |                                                                       |
| **Vectors**       | ✅         | `Vector2`, `Vector2I`, `Vector3`, `Vector3I`, `Vector4`, `Vector4I` |                                                 |
| **Quaternion**    | ✅         | `Quaternion`                                  |                                                                       |
| **NodePath**      | ✅         | `NodePath`                                    |                                                                       |
| **StringName**    | ✅         | `StringName`                                  |                                                                       |
| **Methods**       | ✅         |                                               | Method parameters support all listed types here                       |
| **Static Members**| ✅         |                                               | This includes static methods, fields, and properties                  |
| **Arrays**        | ✅         | `int[]`, `string[]`, `Vector2[]`              | Arrays support all listed types here                                  |
| **Lists**         | ✅         | `List<string[]>`, `List<Vector2>`             | Lists support all listed types here                                   |
| **Dictionaries**  | ✅         | `Dictionary<List<Color[]>, Vector2>`          | Dictionaries support all listed types here                            |
| **Structs**       | ⚠️         |                                               | Appears to work for the most part, needs more testing                 |
| **Classes**       | ⚠️         |                                               | Lots of missing features, things may break if used                    |
| **Records**       | ❌         | `record`                                      | I couldn't get it to work for some reason                             |
| **Godot Classes** | ❌         | `Node`, `PointLight2D`                        | I'm not even sure how this would work                                 |

By annotating your members with `[Visualize]`, you can streamline the debugging process and gain real-time insights into your game's state and behavior.
