namespace Template;

public partial class UICredits : Node
{
    const float STARTING_SPEED = 0.75f;

    VBoxContainer vbox;
    Button btnPause;
    Button btnSpeed;
    bool paused;
    byte curSpeedSetting = 1;
    float speed = STARTING_SPEED;

    public override void _Ready()
    {
        btnPause = GetNode<Button>("%Pause");
        btnSpeed = GetNode<Button>("%Speed");

        vbox = new VBoxContainer
        {
            SizeFlagsVertical = Control.SizeFlags.ShrinkBegin
        };

        // Read the contents from credits.txt and construct the credits
        FileAccess file = 
            FileAccess.Open("res://credits.txt", FileAccess.ModeFlags.Read);

        while (!file.EofReached())
        {
            string line = Tr(file.GetLine());

            int size = 16;

            if (line.Contains("[h1]"))
            {
                size = 32;
                line = line.Replace("[h1]", "");
            }

            if (line.Contains("[h2]"))
            {
                size = 24;
                line = line.Replace("[h2]", "");
            }

            string translatedLine = "";

            foreach (string word in line.Split(' '))
                translatedLine += Tr(word) + " ";

            if (translatedLine.Contains("http"))
                AddTextWithLink(translatedLine);
            else
                if (string.IsNullOrWhiteSpace(translatedLine))
                    vbox.AddChild(new GPadding(0, 10));
                else
                    vbox.AddChild(new GLabel(translatedLine, size));
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
        Vector2 pos = vbox.Position;
        pos.Y -= speed;
        vbox.Position = pos;

        // Go back to the main menu when the credits are finished
        if (pos.Y <= -vbox.Size.Y)
        {
            Global.Services.Get<SceneManager>().SwitchScene("UI/main_menu");
        }

        if (Input.IsActionJustPressed("ui_cancel"))
        {
            Global.Services.Get<SceneManager>().SwitchScene("UI/main_menu");
        }
    }

    void AddTextWithLink(string text)
    {
        int indexOfHttp = text.IndexOf("http");

        string textDesc = text.Substring(0, indexOfHttp);
        string textLink = text.Substring(indexOfHttp);

        HBoxContainer hbox = new HBoxContainer {
            SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter
        };

        GLabel labelText = new GLabel(textDesc);
        GLinkButton btnLink = new GLinkButton(textLink);

        hbox.AddChild(labelText);
        hbox.AddChild(btnLink);

        vbox.AddChild(hbox);
    }

    void _on_pause_pressed()
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

    void _on_speed_pressed()
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
