namespace Template;

public partial class UIPopupMenu : Control
{
    private VBoxContainer vbox;
    private PanelContainer menu;
    private UIOptions options;

    public override void _Ready()
    {
        menu = GetNode<PanelContainer>("Center/Panel");
        vbox = menu.GetNode<VBoxContainer>("Margin/Nav");

        options = Prefabs.Options.Instantiate<UIOptions>();
        AddChild(options);
        options.Hide();
        Hide();
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            if (options.Visible)
            {
                options.Hide();
                menu.Show();
            }
            else
            {
                Visible = !Visible;

                if (Visible)
                {
                    // todo: pause the game
                }
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
        options.Show();
        menu.Hide();
    }

    private void _on_main_menu_pressed()
    {
        AudioManager.PlayMusic(Music.Menu);
        SceneManager.SwitchScene("main_menu");
    }

    private void _on_quit_pressed()
    {
        Global.Quit();
    }
}
