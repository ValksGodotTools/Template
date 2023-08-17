namespace Template;

public partial class UIMainMenuNav : Node
{
    public override void _Ready()
    {
        GetNode<Button>("Play").GrabFocus();
    }

    void _on_play_pressed()
    {
        AudioManager.PlayMusic(Music.Level1, false);
        SceneManager.SwitchScene("level_2D_top_down", SceneManager.TransType.Fade);
    }

    void _on_options_pressed()
    {
        AudioManager.PlayMusic(Music.Level4);
        SceneManager.SwitchScene("Prefabs/UI/options");
    }

    void _on_credits_pressed()
    {
        AudioManager.PlayMusic(Music.Level4);
        SceneManager.SwitchScene("credits");
    }

    void _on_quit_pressed() => 
        GetNode<Global>("/root/Global").Quit();

    void _on_discord_pressed() =>
        OS.ShellOpen("https://discord.gg/866cg8yfxZ");
    void _on_github_pressed() =>
        OS.ShellOpen("https://github.com/ValksGodotTools/Template");
}
