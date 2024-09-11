using Godot;

namespace Template;

public partial class UIMainMenuNav : Node
{
    AudioManager audioManager;

    public override void _Ready()
    {
        audioManager = Global.Services.Get<AudioManager>();

        GetNode<Button>("Play").GrabFocus();
    }

    void _on_play_pressed()
    {
        GD.Print("The play button does not currently go to any scene.");

        //audioManager.PlayMusic(Music.Level1, false);
        //Game.SwitchScene(Scene.SCENEHERE, SceneManager.TransType.Fade);
    }

    void _on_mods_pressed()
    {
        //audioManager.PlayMusic(Music.Level4);
        Game.SwitchScene(Scene.UIModLoader);
    }

    void _on_options_pressed()
    {
        //audioManager.PlayMusic(Music.Level4);
        Game.SwitchScene(Prefab.UIOptions);
    }

    void _on_credits_pressed()
    {
        //audioManager.PlayMusic(Music.Level4);
        Game.SwitchScene(Scene.UICredits);
    }

    async void _on_quit_pressed()
    {
        await GetNode<Global>("/root/Global").QuitAndCleanup();
    }

    void _on_discord_pressed()
    {
        OS.ShellOpen("https://discord.gg/j8HQZZ76r8");
    }

    void _on_github_pressed()
    {
        OS.ShellOpen("https://github.com/ValksGodotTools/Template");
    }
}

