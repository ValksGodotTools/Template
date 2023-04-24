using static Godot.DisplayServer;

namespace Template;

public partial class UIOptionsDisplay : Control
{
    private ResourceOptions Options { get; set; }

    // Max FPS
    private HSlider SliderMaxFPS { get; set; }
    private Label LabelMaxFPSFeedback { get; set; }

    // Window Size
    private LineEdit ResX { get; set; }
    private LineEdit ResY { get; set; }

    private int PrevNumX, PrevNumY;

    // Window Mode
    private OptionButton OptionButtonWindowMode { get; set; }

    // VSync Mode
    private OptionButton OptionButtonVSyncMode { get; set; }

    public override void _Ready()
    {
        Options = OptionsManager.Options;

        SetupMaxFPS();
        SetupWindowSize();
        SetupWindowMode();
        SetupVSyncMode();
    }

    private void SetupMaxFPS()
    {
        LabelMaxFPSFeedback = GetNode<Label>("MaxFPS/HBox/Panel/Margin/MaxFPSFeedback");
        LabelMaxFPSFeedback.Text = Options.MaxFPS == 0 ?
            "UNLIMITED" : Options.MaxFPS + "";

        SliderMaxFPS = GetNode<HSlider>("MaxFPS/HBox/MaxFPS");
        SliderMaxFPS.Value = Options.MaxFPS;

        if (Options.VSyncMode != DisplayServer.VSyncMode.Disabled)
            SliderMaxFPS.Editable = false;
        else
            SliderMaxFPS.Editable = true;
    }

    private void SetupWindowSize()
    {
        ResX = GetNode<LineEdit>("WindowSize/HBox/WindowWidth");
        ResY = GetNode<LineEdit>("WindowSize/HBox/WindowHeight");

        var winSize = DisplayServer.WindowGetSize();

        PrevNumX = winSize.X;
        PrevNumY = winSize.Y;

        ResX.Text = winSize.X + "";
        ResY.Text = winSize.Y + "";
    }

    private void SetupWindowMode()
    {
        OptionButtonWindowMode = GetNode<OptionButton>("WindowMode/WindowMode");
        OptionButtonWindowMode.Select((int)Options.WindowMode);

        OptionsManager.WindowModeChanged += windowMode =>
            OptionButtonWindowMode.Select((int)windowMode);
    }

    private void SetupVSyncMode()
    {
        OptionButtonVSyncMode = GetNode<OptionButton>("VSyncMode/VSyncMode");
        OptionButtonVSyncMode.Select((int)Options.VSyncMode);
    }

    private void ApplyWindowSize()
    {
        DisplayServer.WindowSetSize(new Vector2I(PrevNumX, PrevNumY));

        // Center window
        var winSize = DisplayServer.WindowGetSize();
        DisplayServer.WindowSetPosition(DisplayServer.ScreenGetSize() / 2 - winSize / 2);

        OptionsManager.Options.WindowSize = winSize;
    }

    private void _on_window_mode_item_selected(int index)
    {
        switch ((WindowMode)index)
        {
            case WindowMode.Windowed:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                Options.WindowMode = WindowMode.Windowed;
                break;
            case WindowMode.Borderless:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
                Options.WindowMode = WindowMode.Borderless;
                break;
            case WindowMode.Fullscreen:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
                Options.WindowMode = WindowMode.Fullscreen;
                break;
        }

        // Update UIWindowSize element on window mode change
        var winSize = DisplayServer.WindowGetSize();

        ResX.Text = winSize.X + "";
        ResY.Text = winSize.Y + "";
        PrevNumX = winSize.X;
        PrevNumY = winSize.Y;

        Options.WindowSize = winSize;
    }

    private void _on_window_width_text_changed(string text) =>
        Utils.ValidateNumber(
            text,
            ResX,
            0,
            DisplayServer.ScreenGetSize().X,
            ref PrevNumX);

    private void _on_window_height_text_changed(string text) =>
        Utils.ValidateNumber(
            text,
            ResY,
            0,
            DisplayServer.ScreenGetSize().Y,
            ref PrevNumY);

    private void _on_window_width_text_submitted(string t) => ApplyWindowSize();
    private void _on_window_height_text_submitted(string t) => ApplyWindowSize();
    private void _on_window_size_apply_pressed() => ApplyWindowSize();

    private void _on_v_sync_mode_item_selected(int index)
    {
        var vsyncMode = (DisplayServer.VSyncMode)index;
        DisplayServer.WindowSetVsyncMode(vsyncMode);
        Options.VSyncMode = vsyncMode;

        if (Options.VSyncMode != DisplayServer.VSyncMode.Disabled)
            SliderMaxFPS.Editable = false;
        else
            SliderMaxFPS.Editable = true;
    }

    private void _on_max_fps_value_changed(float value)
    {
        LabelMaxFPSFeedback.Text = value == 0 ?
            "UNLIMITED" : value + "";

        Options.MaxFPS = (int)value;
    }

    private void _on_max_fps_drag_ended(bool valueChanged)
    {
        if (!valueChanged)
            return;

        Engine.MaxFps = Options.MaxFPS;
    }
}

public enum WindowMode
{
    Windowed,
    Borderless,
    Fullscreen
}
