namespace Template;

using Godot.Collections;

// Members of this class were set to static because this will exist for the
// duration of the applications life and there should be no issues with
// using these functions anywhere at anytime.
public partial class OptionsManager : Node
{
    public static event Action<WindowMode> WindowModeChanged;

    public static ResourceOptions Options { get; set; }

    public static Dictionary<StringName, Array<InputEvent>> DefaultHotkeys { get; set; }
    public static ResourceHotkeys Hotkeys { get; set; }

    public static string CurrentOptionsTab { get; set; } = "General";

    public static void SaveOptions()
    {
        Error error = ResourceSaver.Save(
            resource: OptionsManager.Options, 
            path: "user://options.tres");

        if (error != Error.Ok)
            GD.Print(error);
    }

    public static void SaveHotkeys()
    {
        Error error = ResourceSaver.Save(
            resource: OptionsManager.Hotkeys, 
            path: "user://hotkeys.tres");

        if (error != Error.Ok)
            GD.Print(error);
    }

    public static void ResetHotkeys()
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

    public override void _Ready()
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

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("fullscreen"))
        {
            if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Windowed)
            {
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
                Options.WindowMode = WindowMode.Borderless;
                WindowModeChanged?.Invoke(WindowMode.Borderless);
            }
            else
            {
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                Options.WindowMode = WindowMode.Windowed;
                WindowModeChanged?.Invoke(WindowMode.Windowed);
            }
        }
    }

    void LoadOptions()
    {
        bool fileExists = FileAccess.FileExists("user://options.tres");

        Options = fileExists ?
            GD.Load<ResourceOptions>("user://options.tres") : new();
    }

    static void LoadInputMap(Dictionary<StringName, Array<InputEvent>> hotkeys)
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
            LoadInputMap(Hotkeys.Actions);
        }
        else
        {
            Hotkeys = new();
            ResetHotkeys();
        }
    }

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
