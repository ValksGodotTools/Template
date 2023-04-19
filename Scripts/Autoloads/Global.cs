global using Godot;
global using GodotUtils;
global using System;
global using System.Collections.Generic;
global using System.Collections.Concurrent;
global using System.Diagnostics;
global using System.Linq;
global using System.Runtime.CompilerServices;
global using System.Threading;
global using System.Text.RegularExpressions;
global using System.Threading.Tasks;

namespace Template;

public partial class Global : Node
{
    public static event Action<WindowMode> WindowModeChanged;

    public static ResourceOptions Options { get; set; }

    private static Global Instance { get; set; }

	public override void _Ready()
	{
        Instance = this;

        LoadOptions();

        SetWindowMode();
        SetVSyncMode();
        SetWinSize();
        SetMaxFPS();
        SetLanguage();

        // Gradually fade out all SFX whenever the scene is changed
        SceneManager.SceneChanged += name => AudioManager.FadeOutSFX();
    }

	public override void _PhysicsProcess(double delta)
	{
		Logger.Update();
	}

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("fullscreen"))
        {
            if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Windowed)
            {
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
                Options.WindowMode = WindowMode.Borderless;
                WindowModeChanged?.Invoke(WindowMode.Borderless);
            }
            else
            {
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                Options.WindowMode = WindowMode.Windowed;
                WindowModeChanged?.Invoke(WindowMode.Windowed);
            }
        }
    }

    public override void _Notification(int what)
	{
		if (what == NotificationWMCloseRequest)
		{
			GetTree().AutoAcceptQuit = false;
			Quit();
		}
	}

	public static void Quit()
	{
        // Handle cleanup here
        var error = ResourceSaver.Save(Options, "user://options.tres");

        if (error != Error.Ok)
            GD.Print(error);

        Instance.GetTree().Quit();
	}

    private void LoadOptions()
    {
        var fileExists = FileAccess.FileExists("user://options.tres");

        Options = fileExists ?
            GD.Load<ResourceOptions>("user://options.tres") : new();
    }

    private void SetWindowMode()
    {
        switch (Options.WindowMode)
        {
            case WindowMode.Windowed:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                break;
            case WindowMode.Borderless:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
                break;
            case WindowMode.Fullscreen:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
                break;
        }
    }

    private void SetVSyncMode() => DisplayServer.WindowSetVsyncMode(Options.VSyncMode);

    private void SetWinSize()
    {
        if (Options.WindowSize != Vector2I.Zero)
        {
            DisplayServer.WindowSetSize(Options.WindowSize);

            // center window
            var screenSize = DisplayServer.ScreenGetSize();
            var winSize = DisplayServer.WindowGetSize();
            DisplayServer.WindowSetPosition(screenSize / 2 - winSize / 2);
        }
    }

    private void SetMaxFPS()
    {
        if (DisplayServer.WindowGetVsyncMode() == DisplayServer.VSyncMode.Disabled)
        {
            Engine.MaxFps = Options.MaxFPS;
        }
    }

    private void SetLanguage() => TranslationServer.SetLocale(
        Options.Language.ToString().Substring(0, 2).ToLower());
}
