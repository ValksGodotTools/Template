namespace Template;

public partial class UICredits : Node
{
    private const float STARTING_SPEED = 0.75f;

    private VBoxContainer vbox;
    private Button btnPause;
    private Button btnSpeed;
    private bool paused;
    private byte curSpeedSetting = 1;
    private float speed = STARTING_SPEED;

    public override void _Ready()
    {
        btnPause = GetNode<Button>("HBox/Pause");
        btnSpeed = GetNode<Button>("HBox/Speed");

        vbox = new VBoxContainer
        {
            SizeFlagsVertical = Control.SizeFlags.ShrinkBegin
        };

        // Read the contents from credits.txt and construct the credits
        var file = FileAccess.Open("res://credits.txt", FileAccess.ModeFlags.Read);

        while (!file.EofReached())
        {
            var line = Tr(file.GetLine());

            var translatedLine = "";

            foreach (var word in line.Split(' '))
                translatedLine += Tr(word) + " ";

            if (translatedLine.Contains("http"))
                AddTextWithLink(translatedLine);
            else
                if (string.IsNullOrWhiteSpace(translatedLine))
                    vbox.AddChild(new GPadding(0, 10));
                else
                    vbox.AddChild(new GLabel(translatedLine));
        } 

        file.Close();

        AddChild(vbox);

        // Set starting position of the credits
        vbox.Position = new Vector2(
            DisplayServer.WindowGetSize().X / 2 - vbox.Size.X / 2,
            DisplayServer.WindowGetSize().Y);

        // Re-center credits when window size is changed
        GetViewport().SizeChanged += () =>
        {
            vbox.Position = new Vector2(
                DisplayServer.WindowGetSize().X / 2 - vbox.Size.X / 2,
                vbox.Size.Y);
        };
    }

    public override void _PhysicsProcess(double delta)
    {
        // Animate the credits
        var pos = vbox.Position;
        pos.Y -= speed;
        vbox.Position = pos;

        // Go back to the main menu when the credits are finished
        if (pos.Y <= -vbox.Size.Y)
        {
            AudioManager.PlayMusic(Music.Menu);
            SceneManager.SwitchScene("main_menu");
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            AudioManager.PlayMusic(Music.Menu);
            SceneManager.SwitchScene("main_menu");
        }
    }

    private void AddTextWithLink(string text)
    {
        var indexOfHttp = text.IndexOf("http");

        var textDesc = text.Substring(0, indexOfHttp);
        var textLink = text.Substring(indexOfHttp);

        var hbox = new HBoxContainer {
            SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter
        };

        var labelText = new GLabel(textDesc);
        var btnLink = new GLinkButton(textLink);

        hbox.AddChild(labelText);
        hbox.AddChild(btnLink);

        vbox.AddChild(hbox);
    }

    private void _on_pause_pressed()
    {
        paused = !paused;

        if (paused)
        {
            SetPhysicsProcess(false);
            btnPause.Text = "Resume";
        }
        else
        {
            SetPhysicsProcess(true);
            btnPause.Text = "Pause";
        }
    }

    private void _on_speed_pressed()
    {
        if (curSpeedSetting < 3)
        {
            curSpeedSetting++;
            btnSpeed.Text = $"{curSpeedSetting}.0x";
            speed += 1;
        }
        else
        {
            curSpeedSetting = 1;
            btnSpeed.Text = $"{curSpeedSetting}.0x";
            speed = STARTING_SPEED;
        }
    }
}
