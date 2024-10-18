using Godot;
using Template.Valky;

namespace Template.UI;

public partial class MainMenuNav : Node
{
    private AudioManager _audioManager;

    public override void _Ready()
    {
        _audioManager = Services.Get<AudioManager>();

        GetNode<Button>("Play").GrabFocus();
    }

    private static void _on_play_pressed()
    {
        GD.Print("The play button does not currently go to any scene.");

        //audioManager.PlayMusic(Music.Level1, false);
        //Game.SwitchScene(Scene.SCENEHERE, SceneManager.TransType.Fade);
    }

    private static void _on_mods_pressed()
    {
        //audioManager.PlayMusic(Music.Level4);
        Game.SwitchScene(Scene.ModLoader);
    }

    private static void _on_options_pressed()
    {
        //audioManager.PlayMusic(Music.Level4);
        Game.SwitchScene(Scene.Options);
    }

    private static void _on_credits_pressed()
    {
        //audioManager.PlayMusic(Music.Level4);
        Game.SwitchScene(Scene.Credits);
    }

    private async void _on_quit_pressed()
    {
        await GetNode<Global>("/root/Global").QuitAndCleanup();
    }

    private static void _on_discord_pressed()
    {
        OS.ShellOpen("https://discord.gg/j8HQZZ76r8");
    }

    private static void _on_github_pressed()
    {
        OS.ShellOpen("https://github.com/ValksGodotTools/Template");
    }
}

