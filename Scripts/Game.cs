using Godot;
using System;
using Template.Valky;
using Template.UI;
using static Template.SceneManager;

namespace Template;

public partial class Game
{
    public static UIConsole Console { get => Services.Get<UIConsole>(); }

    public static void SwitchScene(string scene, TransType transType = TransType.None)
    {
        Services.Get<SceneManager>().SwitchScene(scene, transType);
    }

    public static void Log(object message, BBColor color = BBColor.Gray)
    {
        Services.Get<Global>().Logger.Log(message, color);
    }

    public static void Log(params object[] objects)
    {
        Services.Get<Global>().Logger.Log(objects);
    }

    public static void LogWarning(object message, BBColor color = BBColor.Orange)
    {
        Services.Get<Global>().Logger.LogWarning(message, color);
    }

    public static void LogErr(Exception e, string hint = null)
    {
        Services.Get<Global>().Logger.LogErr(e, hint);
    }
}
