using Godot;
using GodotUtils;
using System;
using System.Threading.Tasks;

namespace Template;

public partial class Global : Node
{
    public static GameServiceProvider Services { get; } = new();

    /// <summary>
    /// If no await calls are needed, add "return await Task.FromResult(1);"
    /// </summary>
    public event Func<Task> OnQuit;

    [Export] OptionsManager optionsManager;

	public override void _Ready()
	{
        ServiceProvider.Init(Services);
        Services.Add(this);

        Global.Services.Get<Logger>().MessageLogged += Game.Console.AddMessage;

        new ModLoader().LoadMods(this);
    }

	public override void _PhysicsProcess(double delta)
	{
        if (Input.IsActionJustPressed("fullscreen"))
            optionsManager.ToggleFullscreen();
        
        Services.Get<Logger>().Update();
	}

    public override async void _Notification(int what)
	{
		if (what == NotificationWMCloseRequest)
		{
			await QuitAndCleanup();
		}
	}

    public async Task QuitAndCleanup()
	{
        GetTree().AutoAcceptQuit = false;

        // Handle cleanup here
        optionsManager.SaveOptions();
        optionsManager.SaveHotkeys();

        if (OnQuit != null)
            await OnQuit?.Invoke();

        // This must be here because buttons call Global::Quit()
        GetTree().Quit();
	}
}

