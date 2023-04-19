namespace Template;

public partial class UIPopupMenu : Control
{
    public override void _Ready()
    {
        Hide();
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            if (Visible)
            {
                Hide();
            }
            else
            {
                // todo: pause the game
                Show();
            }
        }
    }

    private void _on_resume_pressed()
    {
        // todo: unpause the game
        Hide();
    }

    private void _on_options_pressed()
    {

    }

    private void _on_main_menu_pressed()
    {
        AudioManager.PlayMusic(Songs.Menu);
        SceneManager.SwitchScene("main_menu");
    }

    private void _on_quit_pressed()
    {
        Global.Quit();
    }
}
