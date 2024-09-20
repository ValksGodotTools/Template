using Godot;
using GodotUtils;
using System.Threading.Tasks;
using System;

namespace Template;

public partial class Global : Node
{
    /// <summary>
    /// If no await calls are needed, add "return await Task.FromResult(1);"
    /// </summary>
    public event Func<Task> OnQuit;

    [Export] private OptionsManager optionsManager;

	public override void _Ready()
	{
        ServiceProvider.Services.Add(this);
        ServiceProvider.Services.Get<Logger>().MessageLogged += Game.Console.AddMessage;

        new ModLoader().LoadMods(this);
    }

	public override void _PhysicsProcess(double delta)
	{
        if (Input.IsActionJustPressed("fullscreen"))
        {
            optionsManager.ToggleFullscreen();
        }

        ServiceProvider.Services.Get<Logger>().Update();
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
        {
            await OnQuit?.Invoke();
        }

        // This must be here because buttons call Global::Quit()
        GetTree().Quit();
	}
}

