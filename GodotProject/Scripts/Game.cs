﻿using static Template.SceneManager;

namespace Template;

public partial class Game
{
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

    public static void LogWarning(object message, BBColor color = BBColor.Orange)
    {
        Global.Services.Get<Logger>().LogWarning(message, color);
    }
}
