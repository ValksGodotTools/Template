namespace Template;

using Godot.Collections;

public partial class UIOptionsInput : Control
{
    public static BtnInfo BtnNewInput { get; set; } // the btn waiting for new input
    private Dictionary<StringName, Array<InputEvent>> DefaultActions { get; set; }
    private VBoxContainer Content { get; set; }

    public override void _Ready()
    {
        Content = GetNode<VBoxContainer>("Scroll/VBox");
        CreateHotkeys();
    }

    public override void _Input(InputEvent @event)
    {
        if (BtnNewInput != null)
        {
            if (Input.IsActionJustPressed("remove_hotkey"))
            {
                // Todo: Remove the hotkey action / action events

                BtnNewInput.Btn.QueueFree();
                BtnNewInput = null;
            }

            if (Input.IsActionJustPressed("ui_cancel"))
            {
                BtnNewInput.Btn.Text = BtnNewInput.OriginalText;

                if (BtnNewInput.Plus)
                    BtnNewInput.Btn.QueueFree();

                BtnNewInput = null;
                return;
            }

            // Handle all key inputs
            if (@event is InputEventKey eventKey && !eventKey.Echo)
            {
                // Only check when the last key was released from the keyboard
                if (!eventKey.Pressed)
                {
                    if (eventKey.Keycode == Key.Escape)
                        return;

                    var action = BtnNewInput.Action;

                    // Re-create the button

                    // Preserve the index the button was originally at
                    var index = BtnNewInput.Btn.GetIndex();

                    // Destroy the button
                    BtnNewInput.Btn.QueueFree();

                    // Create the button
                    var btn = CreateButton(action, eventKey, BtnNewInput.HBox);

                    // Move the button to where it was originally at
                    BtnNewInput.HBox.MoveChild(btn, index);

                    var actions = OptionsManager.Hotkeys.Actions;

                    // Clear the specific action event
                    actions[action].Remove(BtnNewInput.InputEventKey);

                    // Update the specific action event
                    actions[action].Add(@event);

                    // Update input map
                    if (BtnNewInput.InputEventKey != null)
                        InputMap.ActionEraseEvent(action, BtnNewInput.InputEventKey);

                    InputMap.ActionAddEvent(action, @event);

                    // No longer waiting for new input
                    BtnNewInput = null;
                }
            }
        }
        else
        {
            if (Input.IsActionJustPressed("ui_cancel"))
            {
                if (SceneManager.CurrentScene.Name == "Options")
                {
                    if (BtnNewInput == null)
                    {
                        AudioManager.PlayMusic(Music.Menu);
                        SceneManager.SwitchScene("main_menu");
                    }
                }
            }
        }
    }

    private Button CreateButton(string action, InputEventKey key, HBoxContainer hbox)
    {
        // Create the button
        var btn = new GButton(key.Readable());
        btn.Pressed += () =>
        {
            // Do not do anything if listening for new input
            if (BtnNewInput != null)
                return;

            // Listening for new hotkey to replace old with...
            BtnNewInput = new BtnInfo
            {
                Action = action,
                Btn = btn,
                HBox = hbox,
                InputEventKey = key,
                OriginalText = btn.Text
            };

            // Give feedback to the user saying we are waiting for new input
            btn.Text = "...";
        };

        // Add this button to the hbox container
        hbox.AddChild(btn);
        return btn;
    }

    private void CreateButtonPlus(string action, HBoxContainer hbox)
    {
        // Create the button
        var btn = new GButton("+");
        btn.Pressed += () =>
        {
            // Do not do anything if listening for new input
            if (BtnNewInput != null)
                return;

            // Listening for new hotkey to replace old with...
            BtnNewInput = new BtnInfo
            {
                Action = action,
                Btn = btn,
                HBox = hbox,
                OriginalText = btn.Text,
                Plus = true
            };

            // Give feedback to the user saying we are waiting for new input
            btn.Text = "...";

            CreateButtonPlus(action, hbox);
        };

        // Add this button to the hbox container
        hbox.AddChild(btn);
    }

    private void CreateHotkeys()
    {
        // Loop through the actions in alphabetical order
        foreach (var action in OptionsManager.Hotkeys.Actions.Keys.OrderBy(x => x.ToString()))
        {
            var actionStr = action.ToString();

            // Exlucde "remove_hotkey" action
            if (actionStr == "remove_hotkey")
                continue;

            // Exclude all built-in actions
            if (actionStr.StartsWith("ui"))
                continue;

            var hbox = new HBoxContainer();

            // For example convert ui_left to UI_LEFT
            var name = action.ToString().ToUpper();

            // Add the action label. For example 'UI Left'
            hbox.AddChild(new GLabel(name)
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                CustomMinimumSize = new Vector2(200, 0)
            });

            // Add all the events after the action label
            var hboxEvents = new HBoxContainer();

            var events = OptionsManager.Hotkeys.Actions[action];

            foreach (var @event in events)
            {
                // Handle keys
                if (@event is InputEventKey eventKey)
                {
                    CreateButton(action, eventKey, hboxEvents);
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

                    hboxEvents.AddChild(btn);
                }
            }

            CreateButtonPlus(action, hboxEvents);

            hbox.AddChild(hboxEvents);
            Content.AddChild(hbox);
        }
    }

    private void _on_reset_to_defaults_pressed()
    {
        for (int i = 0; i < Content.GetChildren().Count; i++)
            if (Content.GetChild(i) != this)
                Content.GetChild(i).QueueFree();

        BtnNewInput = null;
        OptionsManager.ResetHotkeys();
        CreateHotkeys();
    }
}

public class BtnInfo
{
    public InputEventKey InputEventKey { get; set; }
    public string OriginalText { get; set; }
    public StringName Action { get; set; }
    public HBoxContainer HBox { get; set; }
    public Button Btn { get; set; }
    public bool Plus { get; set; }
}
