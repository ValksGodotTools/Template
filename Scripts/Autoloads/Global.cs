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
    public static ResourceOptions Options { get; set; }

    private static Global Instance { get; set; }

	public override void _Ready()
	{
        // Load options from disk
        var fileExists = FileAccess.FileExists("user://options.tres");

        Options = fileExists ? 
            GD.Load<ResourceOptions>("user://options.tres") : new();

        Instance = this;

        SceneManager.SceneChanged += name =>
        {
            // whenever the scene is changed, gradually fade out all SFX
            AudioManager.FadeOutSFX();
        };
    }

	public override void _PhysicsProcess(double delta)
	{
		Logger.Update();
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
}
