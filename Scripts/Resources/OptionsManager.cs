namespace Template;

using Godot.Collections;

// Members of this class were set to static because this will exist for the
// duration of the applications life and there should be no issues with
// using these functions anywhere at anytime.
[GlobalClass]
public partial class OptionsManager : Resource
{
    public event Action<WindowMode> WindowModeChanged;

    public Dictionary<StringName, Array<InputEvent>> DefaultHotkeys { get; set; }
    public ResourceHotkeys Hotkeys { get; private set; }
    public ResourceOptions Options { get; private set; }
    public string CurrentOptionsTab { get; set; } = "General";

    public OptionsManager()
    {
        LoadOptions();

        GetDefaultHotkeys();
        LoadHotkeys();

        SetWindowMode();
        SetVSyncMode();
        SetWinSize();
        SetMaxFPS();
        SetLanguage();
    }

    public void ToggleFullscreen()
    {
        if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Windowed)
        {
            SwitchToFullscreen();
        }
        else
        {
            SwitchToWindow();
        }
    }

    public void SaveOptions()
    {
        Error error = ResourceSaver.Save(
            resource: Options,
            path: "user://options.tres");

        if (error != Error.Ok)
            GD.Print(error);
    }

    public void SaveHotkeys()
    {
        Error error = ResourceSaver.Save(
            resource: Hotkeys,
            path: "user://hotkeys.tres");

        if (error != Error.Ok)
            GD.Print(error);
    }

    public void ResetHotkeys()
    {
        // Deep clone default hotkeys over
        Hotkeys.Actions = new();

        foreach (var element in DefaultHotkeys)
        {
            var arr = new Array<InputEvent>();

            foreach (InputEvent item in DefaultHotkeys[element.Key])
            {
                arr.Add((InputEvent)item.Duplicate());
            }

            Hotkeys.Actions.Add(element.Key, arr);
        }

        // Set input map
        LoadInputMap(DefaultHotkeys);
    }

    void LoadOptions()
    {
        bool fileExists = FileAccess.FileExists("user://options.tres");

        Options = fileExists ?
            GD.Load<ResourceOptions>("user://options.tres") : new();
    }

    void LoadInputMap(Dictionary<StringName, Array<InputEvent>> hotkeys)
    {
        Array<StringName> actions = InputMap.GetActions();

        foreach (StringName action in actions)
            InputMap.EraseAction(action);

        foreach (StringName action in hotkeys.Keys)
        {
            InputMap.AddAction(action);

            foreach (InputEvent @event in hotkeys[action])
                InputMap.ActionAddEvent(action, @event);
        }
    }

    void GetDefaultHotkeys()
    {
        // Get all the default actions defined in the input map
        var actions = new Dictionary<StringName, Array<InputEvent>>();

        foreach (StringName action in InputMap.GetActions())
        {
            actions.Add(action, new Array<InputEvent>());

            foreach (InputEvent actionEvent in InputMap.ActionGetEvents(action))
                actions[action].Add(actionEvent);
        }

        DefaultHotkeys = actions;
    }

    void LoadHotkeys()
    {
        bool fileExists = FileAccess.FileExists("user://hotkeys.tres");

        if (fileExists)
        {
            Hotkeys = GD.Load<ResourceHotkeys>("user://hotkeys.tres");

            // InputMap in project settings has changed so reset all saved hotkeys
            if (!ActionsAreEqual(DefaultHotkeys, Hotkeys.Actions))
            {
                Hotkeys = new();
                ResetHotkeys();
            }

            LoadInputMap(Hotkeys.Actions);
        }
        else
        {
            Hotkeys = new();
            ResetHotkeys();
        }
    }

    bool ActionsAreEqual(Dictionary<StringName, Array<InputEvent>> dict1,
                         Dictionary<StringName, Array<InputEvent>> dict2) =>
        dict1.Count == dict2.Count &&
        dict1.All(pair => dict2.ContainsKey(pair.Key));

    void SetWindowMode()
    {
        switch (Options.WindowMode)
        {
            case WindowMode.Windowed:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                break;
            case WindowMode.Borderless:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
                break;
            case WindowMode.Fullscreen:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
                break;
        }
    }

    void SwitchToFullscreen()
    {
        DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
        Options.WindowMode = WindowMode.Borderless;
        WindowModeChanged?.Invoke(WindowMode.Borderless);
    }

    void SwitchToWindow()
    {
        DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
        Options.WindowMode = WindowMode.Windowed;
        WindowModeChanged?.Invoke(WindowMode.Windowed);
    }

    void SetVSyncMode() => DisplayServer.WindowSetVsyncMode(Options.VSyncMode);

    void SetWinSize()
    {
        if (Options.WindowSize != Vector2I.Zero)
        {
            DisplayServer.WindowSetSize(Options.WindowSize);

            // center window
            Vector2I screenSize = DisplayServer.ScreenGetSize();
            Vector2I winSize = DisplayServer.WindowGetSize();
            DisplayServer.WindowSetPosition(screenSize / 2 - winSize / 2);
        }
    }

    void SetMaxFPS()
    {
        if (DisplayServer.WindowGetVsyncMode() == DisplayServer.VSyncMode.Disabled)
        {
            Engine.MaxFps = Options.MaxFPS;
        }
    }

    void SetLanguage() => TranslationServer.SetLocale(
        Options.Language.ToString().Substring(0, 2).ToLower());
}
