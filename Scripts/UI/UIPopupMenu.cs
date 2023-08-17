namespace Template;

public partial class UIPopupMenu : Control
{
    VBoxContainer vbox;
    PanelContainer menu;
    UIOptions options;

    public override void _Ready()
    {
        menu = GetNode<PanelContainer>("%Menu");
        vbox = GetNode<VBoxContainer>("%Navigation");

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

    void _on_resume_pressed()
    {
        // todo: unpause the game
        Hide();
    }

    void _on_options_pressed()
    {
        options.Show();
        menu.Hide();
    }

    void _on_main_menu_pressed()
    {
        AudioManager.PlayMusic(Music.Menu);
        SceneManager.SwitchScene("main_menu");
    }

    void _on_quit_pressed() => 
        GetNode<Global>("/root/Global").Quit();
}
