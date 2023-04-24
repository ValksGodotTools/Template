namespace Template;

public partial class UIPopupMenu : Control
{
    private VBoxContainer VBox { get; set; }
    private PanelContainer Menu { get; set; }
    private UIOptions Options { get; set; }

    private List<Button> Btns { get; set; } = new();

    public override void _Ready()
    {
        Menu = GetNode<PanelContainer>("Center/Panel");
        VBox = Menu.GetNode<VBoxContainer>("Margin/VBox");

        var resume = new GButton("RESUME");
        resume.Pressed += OnResumePressed;

        var options = new GButton("OPTIONS");
        options.Pressed += OnOptionsPressed;

        var mainMenu = new GButton("MAIN MENU");
        mainMenu.Pressed += OnMainMenuPressed;

        var quit = new GButton("QUIT");
        quit.Pressed += OnQuitPressed;

        Btns.Add(resume);
        Btns.Add(options);
        Btns.Add(mainMenu);
        Btns.Add(quit);

        foreach (var btn in Btns)
            VBox.AddChild(btn);

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

    private void OnResumePressed()
    {
        // todo: unpause the game
        Hide();
    }

    private void OnOptionsPressed()
    {
        Options.Show();
        Menu.Hide();
    }

    private void OnMainMenuPressed()
    {
        AudioManager.PlayMusic(Music.Menu);
        SceneManager.SwitchScene("main_menu");
    }

    private void OnQuitPressed()
    {
        Global.Quit();
    }
}
