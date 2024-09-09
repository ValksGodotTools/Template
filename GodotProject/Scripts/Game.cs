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

    private const int MAX_LABELS_VISIBLE_AT_ONE_TIME = 5;

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

    private static Dictionary<Node, VBoxContainer> visualNodesWithoutVisualAttribute = [];

    public static void Log(object message, Node node, double fadeTime = 5, bool logToConsole = false)
    {
        VBoxContainer vbox = GetOrCreateVBoxContainer(node);

        if (vbox != null)
        {
            AddLabel(vbox, message, fadeTime);
        }

        if (logToConsole)
        {
            Global.Services.Get<Logger>().Log(message, BBColor.Gray);
        }
    }

    private static VBoxContainer GetOrCreateVBoxContainer(Node node)
    {
        if (VisualNodes != null && VisualNodes.TryGetValue(node, out VBoxContainer vbox))
        {
            return vbox;
        }

        if (node is not Control and not Node2D)
        {
            return null;
        }

        if (!visualNodesWithoutVisualAttribute.TryGetValue(node, out vbox))
        {
            vbox = new VBoxContainer
            {
                Scale = Vector2.One * VisualUI.VISUAL_UI_SCALE_FACTOR
            };

            node.AddChild(vbox);
            visualNodesWithoutVisualAttribute[node] = vbox;
        }

        return vbox;
    }

    private static void AddLabel(VBoxContainer vbox, object message, double fadeTime)
    {
        GLabel label = new(message?.ToString());
        label.SetUnshaded();

        vbox.AddChild(label);
        vbox.MoveChild(label, 0);

        if (vbox.GetChildCount() > MAX_LABELS_VISIBLE_AT_ONE_TIME)
        {
            vbox.RemoveChild(vbox.GetChild(vbox.GetChildCount() - 1));
        }

        new GTween(label)
            .SetAnimatingProp(Label.PropertyName.Modulate)
            .AnimateProp(Colors.Transparent, fadeTime)
            .Callback(label.QueueFree);
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

