using Godot;
using GodotUtils;
using System.Collections.Generic;
using System.Linq;
using static Template.SceneManager;

namespace Template;

public partial class Game
{
    public static Net Net { get => Global.Services.Get<Net>(); }
    public static UIConsole Console { get => Global.Services.Get<UIConsole>(); }

    public static Dictionary<Node, VBoxContainer> VisualNodes { get; set; }

    public static void SwitchScene(Scene scene, TransType transType = TransType.None)
    {
        Global.Services.Get<SceneManager>().SwitchScene(scene, transType);
    }

    public static void SwitchScene(Prefab scene, TransType transType = TransType.None)
    {
        Global.Services.Get<SceneManager>().SwitchScene(scene, transType);
    }

    public static T LoadPrefab<T>(Prefab prefab) where T : Node
    {
        return (T)GD.Load<PackedScene>(MapPrefabsToPaths.GetPath(prefab)).Instantiate();
    }

    public static void Log(object message, BBColor color = BBColor.Gray)
    {
        Global.Services.Get<Logger>().Log(message, color);
    }

    public static void Log(object message, Node node, double fadeTime = 5, bool logToConsole = false)
    {
        if (VisualNodes.ContainsKey(node))
        {
            GLabel label = new(message.ToString());
            label.SetUnshaded();

            VBoxContainer vbox = VisualNodes[node];

            vbox.AddChild(label);
            vbox.MoveChild(label, 0);

            int childCount = vbox.GetChildCount();

            const int MAX_LABELS_VISIBLE_AT_ONE_TIME = 5;

            if (childCount > MAX_LABELS_VISIBLE_AT_ONE_TIME)
            {
                vbox.RemoveChild(vbox.GetChild(childCount - 1));
            }

            new GTween(label)
                .SetAnimatingProp(Label.PropertyName.Modulate)
                .AnimateProp(Colors.Transparent, fadeTime)
                .Callback(label.QueueFree);
        }

        if (logToConsole)
        {
            Global.Services.Get<Logger>().Log(message, BBColor.Gray);
        }
    }

    public static void Log(params object[] objects)
    {
        Global.Services.Get<Logger>().Log(objects);
    }

    public static void LogWarning(object message, BBColor color = BBColor.Orange)
    {
        Global.Services.Get<Logger>().LogWarning(message, color);
    }
}

