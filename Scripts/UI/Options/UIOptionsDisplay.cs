using static Godot.DisplayServer;

namespace Template;

public partial class UIOptionsDisplay : Control
{
    private ResourceOptions options;

    // Max FPS
    private HSlider sliderMaxFPS;
    private Label labelMaxFPSFeedback;

    // Window Size
    private LineEdit resX;
    private LineEdit resY;

    private int prevNumX, prevNumY;

    // Window Mode
    private OptionButton optionBtnWindowMode;

    // VSync Mode
    private OptionButton optionBtnVSyncMode;

    public override void _Ready()
    {
        options = OptionsManager.Options;
        SetupMaxFPS();
        SetupWindowSize();
        SetupWindowMode();
        SetupVSyncMode();
    }

    private void SetupMaxFPS()
    {
        labelMaxFPSFeedback = GetNode<Label>("MaxFPS/HBox/Panel/Margin/MaxFPSFeedback");
        labelMaxFPSFeedback.Text = options.MaxFPS == 0 ?
            "UNLIMITED" : options.MaxFPS + "";

        sliderMaxFPS = GetNode<HSlider>("MaxFPS/HBox/MaxFPS");
        sliderMaxFPS.Value = options.MaxFPS;

        if (options.VSyncMode != DisplayServer.VSyncMode.Disabled)
            sliderMaxFPS.Editable = false;
        else
            sliderMaxFPS.Editable = true;
    }

    private void SetupWindowSize()
    {
        resX = GetNode<LineEdit>("WindowSize/HBox/WindowWidth");
        resY = GetNode<LineEdit>("WindowSize/HBox/WindowHeight");

        var winSize = DisplayServer.WindowGetSize();

        prevNumX = winSize.X;
        prevNumY = winSize.Y;

        resX.Text = winSize.X + "";
        resY.Text = winSize.Y + "";
    }

    private void SetupWindowMode()
    {
        optionBtnWindowMode = GetNode<OptionButton>("WindowMode/WindowMode");
        optionBtnWindowMode.Select((int)options.WindowMode);

        OptionsManager.WindowModeChanged += windowMode =>
            // Window mode select button could be null. If there was no null check
            // here then we would be assuming that the user can only change fullscreen
            // when in the options screen but this is not the case.
            optionBtnWindowMode?.Select((int)windowMode);
    }

    private void SetupVSyncMode()
    {
        optionBtnVSyncMode = GetNode<OptionButton>("VSyncMode/VSyncMode");
        optionBtnVSyncMode.Select((int)options.VSyncMode);
    }

    private void ApplyWindowSize()
    {
        DisplayServer.WindowSetSize(new Vector2I(prevNumX, prevNumY));

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
                options.WindowMode = WindowMode.Windowed;
                break;
            case WindowMode.Borderless:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
                options.WindowMode = WindowMode.Borderless;
                break;
            case WindowMode.Fullscreen:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
                options.WindowMode = WindowMode.Fullscreen;
                break;
        }

        // Update UIWindowSize element on window mode change
        var winSize = DisplayServer.WindowGetSize();

        resX.Text = winSize.X + "";
        resY.Text = winSize.Y + "";
        prevNumX = winSize.X;
        prevNumY = winSize.Y;

        options.WindowSize = winSize;
    }

    private void _on_window_width_text_changed(string text) =>
        Utils.ValidateNumber(
            text,
            resX,
            0,
            DisplayServer.ScreenGetSize().X,
            ref prevNumX);

    private void _on_window_height_text_changed(string text) =>
        Utils.ValidateNumber(
            text,
            resY,
            0,
            DisplayServer.ScreenGetSize().Y,
            ref prevNumY);

    private void _on_window_width_text_submitted(string t) => ApplyWindowSize();
    private void _on_window_height_text_submitted(string t) => ApplyWindowSize();
    private void _on_window_size_apply_pressed() => ApplyWindowSize();

    private void _on_v_sync_mode_item_selected(int index)
    {
        var vsyncMode = (DisplayServer.VSyncMode)index;
        DisplayServer.WindowSetVsyncMode(vsyncMode);
        options.VSyncMode = vsyncMode;

        if (options.VSyncMode != DisplayServer.VSyncMode.Disabled)
            sliderMaxFPS.Editable = false;
        else
            sliderMaxFPS.Editable = true;
    }

    private void _on_max_fps_value_changed(float value)
    {
        labelMaxFPSFeedback.Text = value == 0 ?
            "UNLIMITED" : value + "";

        options.MaxFPS = (int)value;
    }

    private void _on_max_fps_drag_ended(bool valueChanged)
    {
        if (!valueChanged)
            return;

        Engine.MaxFps = options.MaxFPS;
    }
}

public enum WindowMode
{
    Windowed,
    Borderless,
    Fullscreen
}
