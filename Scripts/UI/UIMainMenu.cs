namespace Template;

public partial class UIMainMenu : Node
{
    private void _on_play_pressed()
    {
        AudioManager.PlayMusic(Music.Level1, false);
        SceneManager.SwitchScene("level", SceneManager.TransType.Fade);
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

    private void _on_quit_pressed()
    {
        Global.Quit();
    }
}
