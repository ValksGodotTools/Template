using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using Visualize.Utils;

namespace Visualize.Example;

public partial class VisualizeExample : Sprite2D
{
    private readonly VisualLogger logger = new();

    [Visualize]
    public void PrintArray(Array<int> genericArrayInt)
    {
        logger.Log(genericArrayInt.ToFormattedString(), this);
    }
}
