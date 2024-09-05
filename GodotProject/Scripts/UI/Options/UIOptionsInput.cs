using CSharpUtils;
using Godot;
using GodotUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Template;

using Godot.Collections;

public partial class UIOptionsInput : Control
{
    [Export] OptionsManager optionsManager;

    static BtnInfo btnNewInput; // the btn waiting for new input
    Dictionary<StringName, Array<InputEvent>> defaultActions;
    VBoxContainer content;

    public override void _Ready()
    {
        content = GetNode<VBoxContainer>("Scroll/VBox");
        CreateHotkeys();
    }

    public override void _Input(InputEvent @event)
    {
        if (btnNewInput != null)
        {
            if (Input.IsActionJustPressed("remove_hotkey"))
            {
                StringName action = btnNewInput.Action;

                // Update input map
                InputMap.ActionEraseEvent(action, btnNewInput.InputEvent);

                // Update options
                optionsManager.Hotkeys.Actions[action].Remove(btnNewInput.InputEvent);

                // Update UI
                btnNewInput.Btn.QueueFree();
                btnNewInput = null;
            }

            if (Input.IsActionJustPressed("ui_cancel"))
            {
                btnNewInput.Btn.Text = btnNewInput.OriginalText;
                btnNewInput.Btn.Disabled = false;

                if (btnNewInput.Plus)
                    btnNewInput.Btn.QueueFree();

                btnNewInput = null;
                @event.Dispose(); // Object count was increasing a lot when this function was executed
                return;
            }

            if (@event is InputEventMouseButton eventMouseBtn)
            {
                if (!eventMouseBtn.Pressed)
                    HandleInput(eventMouseBtn);
            }
            else if (@event is InputEventKey eventKey && !eventKey.Echo)
            {
                // Only check when the last key was released from the keyboard
                if (!eventKey.Pressed)
                    HandleInput(eventKey);
            }   
        }
        else
        {
            if (Input.IsActionJustPressed("ui_cancel"))
            {
                if (Global.Services.Get<SceneManager>().CurrentScene.Name == "Options")
                {
                    if (btnNewInput == null)
                    {
                        Game.SwitchScene(Scene.UIMainMenu);
                    }
                }
            }
        }

        @event.Dispose(); // Object count was increasing a lot when this function was executed
    }

    void HandleInput(InputEvent @event)
    {
        StringName action = btnNewInput.Action;

        // Prevent something very evil from happening!
        if (action == "fullscreen" && @event is InputEventMouseButton eventBtn)
            return;

        // Re-create the button

        // Preserve the index the button was originally at
        int index = btnNewInput.Btn.GetIndex();

        // Destroy the button
        btnNewInput.Btn.QueueFree();

        // Create the button
        Button btn = CreateButton(action, @event, btnNewInput.HBox);
        btn.Disabled = false;

        // Move the button to where it was originally at
        btnNewInput.HBox.MoveChild(btn, index);

        Dictionary<StringName, Array<InputEvent>> actions = optionsManager.Hotkeys.Actions;

        // Clear the specific action event
        actions[action].Remove(btnNewInput.InputEvent);

        // Update the specific action event
        actions[action].Add(@event);

        // Update input map
        if (btnNewInput.InputEvent != null)
            InputMap.ActionEraseEvent(action, btnNewInput.InputEvent);

        InputMap.ActionAddEvent(action, @event);

        // No longer waiting for new input
        btnNewInput = null;
    }

    Button CreateButton(string action, InputEvent inputEvent, HBoxContainer hbox)
    {
        string readable = "";

        if (inputEvent is InputEventKey key)
        {
            readable = key.Readable();
        }
        else if (inputEvent is InputEventMouseButton button)
        {
            readable = $"Mouse {button.ButtonIndex}";
        }

        // Create the button
        GButton btn = new(readable);
        btn.Pressed += () =>
        {
            // Do not do anything if listening for new input
            if (btnNewInput != null)
                return;

            // Listening for new hotkey to replace old with...
            btnNewInput = new BtnInfo
            {
                Action = action,
                Btn = btn,
                HBox = hbox,
                InputEvent = inputEvent,
                OriginalText = btn.Text
            };

            // Give feedback to the user saying we are waiting for new input
            btn.Disabled = true;
            btn.Text = "...";
        };

        // Add this button to the hbox container
        hbox.AddChild(btn);
        return btn;
    }

    void CreateButtonPlus(string action, HBoxContainer hbox)
    {
        // Create the button
        GButton btn = new("+");
        btn.Pressed += () =>
        {
            // Do not do anything if listening for new input
            if (btnNewInput != null)
                return;

            // Listening for new hotkey to replace old with...
            btnNewInput = new BtnInfo
            {
                Action = action,
                Btn = btn,
                HBox = hbox,
                OriginalText = btn.Text,
                Plus = true
            };

            // Give feedback to the user saying we are waiting for new input
            btn.Disabled = true;
            btn.Text = "...";

            CreateButtonPlus(action, hbox);
        };

        // Add this button to the hbox container
        hbox.AddChild(btn);
    }

    void CreateHotkeys()
    {
        // Loop through the actions in alphabetical order
        foreach (StringName action in optionsManager.Hotkeys.Actions.Keys.OrderBy(x => x.ToString()))
        {
            string actionStr = action.ToString();

            // Exlucde "remove_hotkey" action
            if (actionStr == "remove_hotkey")
                continue;

            // Exclude all built-in actions
            if (actionStr.StartsWith("ui"))
                continue;

            HBoxContainer hbox = new();

            // For example convert ui_left to UI_LEFT
            string name = action.ToString().ToUpper();

            // Add the action label. For example 'UI Left'
            hbox.AddChild(new GLabel(name)
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                CustomMinimumSize = new Vector2(200, 0)
            });

            // Add all the events after the action label
            HBoxContainer hboxEvents = new();

            Array<InputEvent> events = optionsManager.Hotkeys.Actions[action];

            foreach (InputEvent @event in events)
            {
                // Handle keys
                if (@event is InputEventKey eventKey)
                {
                    CreateButton(action, eventKey, hboxEvents);
                }

                // Handle mouse buttons
                if (@event is InputEventMouseButton eventMouseBtn)
                {
                    CreateButton(action, eventMouseBtn, hboxEvents);
                }
            }

            CreateButtonPlus(action, hboxEvents);

            hbox.AddChild(hboxEvents);
            content.AddChild(hbox);
        }
    }

    void _on_reset_to_defaults_pressed()
    {
        for (int i = 0; i < content.GetChildren().Count; i++)
            if (content.GetChild(i) != this)
                content.GetChild(i).QueueFree();

        btnNewInput = null;
        optionsManager.ResetHotkeys();
        CreateHotkeys();
        GC.Collect(); // Object count was increasing a lot when this function was executed
    }
}

public class BtnInfo
{
    public InputEvent InputEvent { get; set; }
    public string OriginalText { get; set; }
    public StringName Action { get; set; }
    public HBoxContainer HBox { get; set; }
    public Button Btn { get; set; }
    public bool Plus { get; set; }
}

