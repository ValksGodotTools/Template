﻿namespace Template;

public partial class Game
{
    public static SceneManager SceneManager { get => Global.Services.Get<SceneManager>(); }

    public static void Log(object message, BBColor color = BBColor.Gray) =>
        Global.Services.Get<Logger>().Log(message, color);

    public static void LogWarning(object message, BBColor color = BBColor.Orange) =>
        Global.Services.Get<Logger>().LogWarning(message, color);

    static partial void HelloFrom(string name);

    public static void Test(string name) => HelloFrom(name);
}