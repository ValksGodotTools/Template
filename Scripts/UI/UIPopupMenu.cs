namespace Template;

public partial class UIPopupMenu : Control
{
    private VBoxContainer  VBox    { get; set; }
    private PanelContainer Menu    { get; set; }
    private UIOptions      Options { get; set; }

    public override void _Ready()
    {
        Menu = GetNode<PanelContainer>("Center/Panel");
        VBox = Menu.GetNode<VBoxContainer>("Margin/Nav");

        Options = Prefabs.Options.Instantiate<UIOptions>();
        AddChild(Options);
        Options.Hide();
        Hide();
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            if (Options.Visible)
            {
                Options.Hide();
                Menu.Show();
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
        Options.Show();
        Menu.Hide();
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
