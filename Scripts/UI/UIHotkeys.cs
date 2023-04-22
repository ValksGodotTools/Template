namespace Template;

public partial class UIHotkeys : Node
{
    private Node Parent             { get; set; }
    private bool WaitingForNewInput { get; set; }

    public override void _Ready()
    {
        Parent = GetParent();
        CreateHotkeys();
    }

    public override void _Input(InputEvent @event)
    {
        if (!WaitingForNewInput)
            return;

        if (@event is InputEventKey eventKey && !eventKey.Echo)
        {
            if (!eventKey.Pressed)
            {
                WaitingForNewInput = false;
                GD.Print(OS.GetKeycodeString(eventKey.GetKeycodeWithModifiers()));

                // TODO: Set to new eventKey

                //var curActions = InputMap.ActionGetEvents(actionName);

                //InputMap.ActionEraseEvents(actionName);

                //InputMap.ActionAddEvent(actionName, inputEvent);
            }
        }
    }

    private void CreateHotkeys()
    {
        // The important UI actions we care about
        var uiActions = new string[] {
            "ui_left",
            "ui_down",
            "ui_right",
            "ui_up",
            "ui_select",
            "ui_cancel",
            "ui_accept"
        };

        foreach (var action in InputMap.GetActions())
        {
            var actionStr = action.ToString();

            // Exclude all the UI actions we do not care about
            if (actionStr.StartsWith("ui") && !uiActions.Contains(actionStr))
                continue;

            var hbox = new HBoxContainer();

            // For example convert ui_left to UI Left
            var words = action.ToString().Split('_');
            for (int i = 0; i < words.Length; i++)
            {
                // Convert first letter to be uppercase
                words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1);

                // Convert special words to all uppercase
                if (words[i] == "Ui")
                    words[i] = words[i].ToUpper();
            }

            // Add the action label. For example 'UI Left'
            hbox.AddChild(new GLabel(words.Join(" "))
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                CustomMinimumSize = new Vector2(200, 0)
            });

            // Add all the events after the action label
            var events = InputMap.ActionGetEvents(action);

            foreach (var @event in events)
            {
                // Handle keys
                if (@event is InputEventKey eventKey)
                {
                    // If Keycode is not set than use PhysicalKeycode
                    var keyWithModifiers = eventKey.Keycode == Key.None ?
                        eventKey.GetPhysicalKeycodeWithModifiers() :
                        eventKey.GetKeycodeWithModifiers();

                    // Convert to a human readable key
                    // For example 'Ctrl + Shift + E'
                    var readableKey = OS.GetKeycodeString(keyWithModifiers);
                    readableKey = readableKey.Replace("+", " + ");

                    // Create the button
                    var btn = new GButton(readableKey);
                    btn.Pressed += () =>
                    {
                        // Do not do anything is listening for new input
                        if (WaitingForNewInput)
                            return;

                        // Listening for new hotkey to replace old with...
                        WaitingForNewInput = true;
                        btn.Text = "...";
                    };

                    // Add the button
                    hbox.AddChild(btn);
                }

                // Handle mouse buttons
                if (@event is InputEventMouseButton eventMouseBtn)
                {
                    // WIP / TODO

                    var btn = new GButton(eventMouseBtn.ButtonIndex + "");
                    btn.Pressed += () =>
                    {
                        btn.Text = "...";
                    };

                    hbox.AddChild(btn);
                }
            }

            Parent.AddChild(hbox);
        }
    }
}
