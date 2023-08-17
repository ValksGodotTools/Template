namespace GodotUtils;

public partial class UIConsole : PanelContainer
{
    public static bool IsVisible { get => instance.Visible; }

    static TextEdit feed;
    LineEdit input;
    Button settingsBtn;
    PopupPanel settingsPopup;
    CheckBox settingsAutoScroll;
    readonly ConsoleHistory history = new();
    static bool autoScroll = true;
    static UIConsole instance;

    public override void _Ready()
    {
        instance = this;

        feed          = GetNode<TextEdit>("%Output");
        input         = GetNode<LineEdit>("%Input");
        settingsBtn   = GetNode<Button>  ("%Settings");
        settingsPopup = GetNode<PopupPanel>("PopupPanel");
        
        var settingsVBox = GetNode("%PopupVBox");
        settingsAutoScroll = GetNode<CheckBox>("%PopupAutoScroll");

        input.TextSubmitted += OnConsoleInputEntered;
        settingsBtn.Pressed += OnSettingsBtnPressed;
        settingsAutoScroll.Toggled += v => autoScroll = v;
        settingsAutoScroll.ButtonPressed = autoScroll;

        Hide();
    }

    public override async void _Input(InputEvent @event)
    {
        await InputVisibility(@event);
        InputNavigateHistory();
    }

    public static void AddMessage(object message)
    {
        // UIConsole was not set up properly so return
        if (instance == null)
            return;

        var prevScroll = feed.ScrollVertical;
        
        // Prevent text feed from becoming too large
        if (feed.Text.Count() > 1000)
            // If there are say 2353 characters then 2353 - 1000 = 1353 characters
            // which is how many characters we need to remove to get back down to
            // 1000 characters
            feed.Text = feed.Text.Remove(0, feed.Text.Count() - 1000);
        
        feed.Text += $"\n{message}";

        // Removing text from the feed will mess up the scroll, this is why the
        // scroll value was stored previous, we set this to that value now to fix
        // this
        feed.ScrollVertical = prevScroll;

        // Autoscroll if enabled
        ScrollDown();
    }

    static void ScrollDown()
    {
        if (autoScroll)
            feed.ScrollVertical = (int)feed.GetVScrollBar().MaxValue;
    }

    void OnSettingsBtnPressed()
    {
        if (!settingsPopup.Visible)
            settingsPopup.PopupCentered();
    }

    void OnConsoleInputEntered(string text)
    {
        // case sensitivity and trailing spaces should not factor in here
        var inputToLowerTrimmed = text.Trim().ToLower();
        var inputArr = inputToLowerTrimmed.Split(' ');

        // extract command from input
        var cmd = inputArr[0];

        // do not do anything if cmd is just whitespace
        if (string.IsNullOrWhiteSpace(cmd))
            return;

        // keep track of input history
        history.Add(inputToLowerTrimmed);

        // check to see if the command is valid
        var command = Command.Instances.FirstOrDefault(x => x.IsMatch(cmd));

        if (command != null)
        {
            // extract cmd args from input
            var cmdArgs = inputArr.Skip(1).ToArray();

            // run the command
            command.Run(GetTree().Root, cmdArgs);
        }
        else
            // command does not exist
            Logger.Log($"The command '{cmd}' does not exist");

        // clear the input after the command is executed
        input.Clear();
    }

    async Task InputVisibility(InputEvent @event)
    {
        if (@event is not InputEventKey inputEventKey)
            return;

        if (inputEventKey.IsJustPressed(Key.F12))
        {
            await ToggleVisibility();
            return;
        }
    }

    void InputNavigateHistory()
    {
        // If console is not visible or there is no history to navigate do nothing
        if (!Visible || history.NoHistory())
            return;

        if (Godot.Input.IsActionJustPressed("ui_up"))
        {
            var historyText = history.MoveUpOne();

            input.Text = historyText;

            // if deferred is not use then something else will override these settings
            input.CallDeferred("grab_focus");
            input.CallDeferred("set", "caret_column", historyText.Length);
        }

        if (Godot.Input.IsActionJustPressed("ui_down"))
        {
            var historyText = history.MoveDownOne();

            input.Text = historyText;

            // if deferred is not use then something else will override these settings
            input.CallDeferred("grab_focus");
            input.CallDeferred("set", "caret_column", historyText.Length);
        }
    }

    async Task ToggleVisibility()
    {
        Visible = !Visible;

        if (Visible)
        {
            input.GrabFocus();

            // This isn't ideal as you can see the scroll spas out for 1 frame
            // But it is all I can think of doing right now
            // If no task delay is set here then scroll down will not register
            // at all
            await Task.Delay(1);

            ScrollDown();
        }
    }
}
