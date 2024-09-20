using Godot;
using GodotUtils;
using static Template.SceneManager;

namespace Template;

public partial class Game
{
    public static UIConsole Console { get => ServiceProvider.Services.Get<UIConsole>(); }

    public static void SwitchScene(Scene scene, TransType transType = TransType.None)
    {
        ServiceProvider.Services.Get<SceneManager>().SwitchScene(scene, transType);
    }

    public static void SwitchScene(Prefab scene, TransType transType = TransType.None)
    {
        ServiceProvider.Services.Get<SceneManager>().SwitchScene(scene, transType);
    }

    public static T LoadPrefab<T>(Prefab prefab) where T : Node
    {
        return (T)GD.Load<PackedScene>(MapPrefabsToPaths.GetPath(prefab)).Instantiate();
    }

    public static void Log(object message, BBColor color = BBColor.Gray)
    {
        ServiceProvider.Services.Get<Logger>().Log(message, color);
    }

    public static void Log(params object[] objects)
    {
        ServiceProvider.Services.Get<Logger>().Log(objects);
    }

    public static void LogWarning(object message, BBColor color = BBColor.Orange)
    {
        ServiceProvider.Services.Get<Logger>().LogWarning(message, color);
    }
}
