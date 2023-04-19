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
    private static Global Instance { get; set; }

	public override void _Ready()
	{
        Instance = this;

        // Gradually fade out all SFX whenever the scene is changed
        SceneManager.SceneChanged += name => AudioManager.FadeOutSFX();
    }

	public override void _PhysicsProcess(double delta)
	{
		Logger.Update();
	}

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            var curScene = SceneManager.CurrentScene.Name;

            switch (curScene)
            {
                case "Options":
                case "Credits":
                    AudioManager.PlayMusic(Music.Menu);
                    SceneManager.SwitchScene("main_menu");
                    break;
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
        OptionsManager.SaveOptions();

        Instance.GetTree().Quit();
	}
}
