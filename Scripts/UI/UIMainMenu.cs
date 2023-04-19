namespace Template;

public partial class UIMainMenu : Node
{
    public override void _Ready()
    {
        
    }

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

    private void _on_quit_pressed()
    {
        Global.Quit();
    }
}
