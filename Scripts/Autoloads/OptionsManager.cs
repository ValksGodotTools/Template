namespace Template;

public partial class OptionsManager : Node
{
    public static event Action<WindowMode> WindowModeChanged;

    public static ResourceOptions Options { get; set; }

    public static void SaveOptions()
    {
        var error = ResourceSaver.Save(OptionsManager.Options, "user://options.tres");

        if (error != Error.Ok)
            GD.Print(error);
    }

    public override void _Ready()
    {
        LoadOptions();

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

    private void LoadOptions()
    {
        var fileExists = FileAccess.FileExists("user://options.tres");

        Options = fileExists ?
            GD.Load<ResourceOptions>("user://options.tres") : new();
    }

    private void SetWindowMode()
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

    private void SetVSyncMode() => DisplayServer.WindowSetVsyncMode(Options.VSyncMode);

    private void SetWinSize()
    {
        if (Options.WindowSize != Vector2I.Zero)
        {
            DisplayServer.WindowSetSize(Options.WindowSize);

            // center window
            var screenSize = DisplayServer.ScreenGetSize();
            var winSize = DisplayServer.WindowGetSize();
            DisplayServer.WindowSetPosition(screenSize / 2 - winSize / 2);
        }
    }

    private void SetMaxFPS()
    {
        if (DisplayServer.WindowGetVsyncMode() == DisplayServer.VSyncMode.Disabled)
        {
            Engine.MaxFps = Options.MaxFPS;
        }
    }

    private void SetLanguage() => TranslationServer.SetLocale(
        Options.Language.ToString().Substring(0, 2).ToLower());
}
