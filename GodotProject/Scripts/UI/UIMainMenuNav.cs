using Godot;

namespace Template;

public partial class UIMainMenuNav : Node
{
    private AudioManager _audioManager;

    public override void _Ready()
    {
        _audioManager = Global.Services.Get<AudioManager>();

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
        Game.SwitchScene(Scene.UIModLoader);
    }

    private static void _on_options_pressed()
    {
        //audioManager.PlayMusic(Music.Level4);
        Game.SwitchScene(Prefab.UIOptions);
    }

    private static void _on_credits_pressed()
    {
        //audioManager.PlayMusic(Music.Level4);
        Game.SwitchScene(Scene.UICredits);
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

