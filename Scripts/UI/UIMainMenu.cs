namespace Template;

public partial class UIMainMenu : Node
{
    private void _on_play_pressed()
    {
        AudioManager.PlayMusic(Music.Level1, false);
        SceneManager.SwitchScene("level_2D_top_down", SceneManager.TransType.Fade);
    }

    private void _on_options_pressed()
    {
        AudioManager.PlayMusic(Music.Level4);
        SceneManager.SwitchScene("Prefabs/options");
    }

    private void _on_credits_pressed()
    {
        AudioManager.PlayMusic(Music.Level4);
        SceneManager.SwitchScene("credits");
    }

    private void _on_discord_pressed() => OS.ShellOpen("https://discord.gg/866cg8yfxZ");
    private void _on_github_pressed() => OS.ShellOpen("https://github.com/ValksGodotTools/Template");

    private void _on_quit_pressed()
    {
        Global.Quit();
    }
}
