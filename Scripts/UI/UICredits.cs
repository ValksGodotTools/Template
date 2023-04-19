namespace Template;

public partial class UICredits : Node
{
    private VBoxContainer VBox     { get; set; }
    private Button        BtnPause { get; set; }
    private Button        BtnSpeed { get; set; }

    private bool          Paused   { get; set; }
    private float         Speed    { get; set; } = 1.0f;

    public override void _Ready()
    {
        BtnPause = GetNode<Button>("HBox/Pause");
        BtnSpeed = GetNode<Button>("HBox/Speed");

        VBox = new VBoxContainer
        {
            SizeFlagsVertical = Control.SizeFlags.ShrinkBegin
        };

        var file = FileAccess.Open("res://credits.txt", FileAccess.ModeFlags.Read);

        while (!file.EofReached())
        {
            var line = file.GetLine();

            if (line.Contains("http"))
                AddTextWithLink(line);
            else
                if (string.IsNullOrWhiteSpace(line))
                    VBox.AddChild(new GPadding(0, 10));
                else
                    VBox.AddChild(new GLabel(line));
        } 

        file.Close();

        AddChild(VBox);

        VBox.Position = new Vector2(
            DisplayServer.WindowGetSize().X / 2 - VBox.Size.X / 2,
            DisplayServer.WindowGetSize().Y);
    }

    public override void _PhysicsProcess(double delta)
    {
        var pos = VBox.Position;
        pos.Y -= Speed;
        VBox.Position = pos;

        if (pos.Y <= -VBox.Size.Y)
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

        var hbox = new HBoxContainer
        {
            SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter
        };

        var labelText = new GLabel(textDesc);
        var btnLink = new GLinkButton(textLink);

        hbox.AddChild(labelText);
        hbox.AddChild(btnLink);

        VBox.AddChild(hbox);
    }

    private void _on_pause_pressed()
    {
        Paused = !Paused;

        if (Paused)
        {
            SetPhysicsProcess(false);
            BtnPause.Text = "Resume";
        }
        else
        {
            SetPhysicsProcess(true);
            BtnPause.Text = "Pause";
        }
    }

    private void _on_speed_pressed()
    {
        var numSpeeds = 3;

        for (int i = 1; i < numSpeeds; i++)
            if (Speed == i)
            {
                BtnSpeed.Text = $"{i + 1}.0x";
                Speed = i + 1;
                return;
            }

        if (Speed == numSpeeds)
        {
            BtnSpeed.Text = "1.0x";
            Speed = 1;
        }
    }
}
