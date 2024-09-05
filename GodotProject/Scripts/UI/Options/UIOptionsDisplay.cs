using Godot;
using GodotUtils;
using System;

using static Godot.DisplayServer;

namespace Template;

public partial class UIOptionsDisplay : Control
{
    public event Action<int> OnResolutionChanged;

    [Export] OptionsManager optionsManager;

    ResourceOptions options;

    // Max FPS
    HSlider sliderMaxFPS;
    Label labelMaxFPSFeedback;

    // Window Size
    LineEdit resX, resY;

    int prevNumX, prevNumY;
    int min_resolution = 36;

    public override void _Ready()
    {
        options = optionsManager.Options;
        SetupMaxFPS();
        SetupWindowSize();
        SetupWindowMode();
        SetupResolution();
        SetupVSyncMode();
    }

    void SetupMaxFPS()
    {
        labelMaxFPSFeedback = GetNode<Label>("%MaxFPSFeedback");
        labelMaxFPSFeedback.Text = options.MaxFPS == 0 ?
            "UNLIMITED" : options.MaxFPS + "";

        sliderMaxFPS = GetNode<HSlider>("%MaxFPS");
        sliderMaxFPS.Value = options.MaxFPS;
        sliderMaxFPS.Editable = options.VSyncMode == VSyncMode.Disabled;
    }

    void SetupWindowSize()
    {
        resX = GetNode<LineEdit>("%WindowWidth");
        resY = GetNode<LineEdit>("%WindowHeight");

        Vector2I winSize = DisplayServer.WindowGetSize();

        prevNumX = winSize.X;
        prevNumY = winSize.Y;

        resX.Text = winSize.X + "";
        resY.Text = winSize.Y + "";
    }

    void SetupWindowMode()
    {
        OptionButton optionBtnWindowMode = GetNode<OptionButton>("%WindowMode");
        optionBtnWindowMode.Select((int)options.WindowMode);

        optionsManager.WindowModeChanged += windowMode =>
        {
            if (!GodotObject.IsInstanceValid(optionBtnWindowMode))
                return;

            // Window mode select button could be null. If there was no null check
            // here then we would be assuming that the user can only change fullscreen
            // when in the options screen but this is not the case.
            optionBtnWindowMode?.Select((int)windowMode);
        };
    }

    void SetupResolution()
    {
        GetNode<HSlider>("%Resolution").Value = 1 + min_resolution - options.Resolution;
    }

    void SetupVSyncMode()
    {
        GetNode<OptionButton>("%VSyncMode").Select((int)options.VSyncMode);
    }

    void ApplyWindowSize()
    {
        DisplayServer.WindowSetSize(new Vector2I(prevNumX, prevNumY));

        // Center window
        Vector2I winSize = DisplayServer.WindowGetSize();
        DisplayServer.WindowSetPosition(DisplayServer.ScreenGetSize() / 2 - winSize / 2);

        options.WindowSize = winSize;
    }

    void _on_window_mode_item_selected(int index)
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
        Vector2I winSize = DisplayServer.WindowGetSize();

        resX.Text = winSize.X + "";
        resY.Text = winSize.Y + "";
        prevNumX = winSize.X;
        prevNumY = winSize.Y;

        options.WindowSize = winSize;
    }

    void _on_window_width_text_changed(string text) =>
        text.ValidateNumber(resX, 0, ScreenGetSize().X, ref prevNumX);

    void _on_window_height_text_changed(string text) =>
        text.ValidateNumber(resY, 0, ScreenGetSize().Y, ref prevNumY);

    void _on_window_width_text_submitted(string t) => ApplyWindowSize();
    void _on_window_height_text_submitted(string t) => ApplyWindowSize();
    void _on_window_size_apply_pressed() => ApplyWindowSize();

    void _on_resolution_value_changed(float value)
    {
        options.Resolution = min_resolution - (int)value + 1;
        OnResolutionChanged?.Invoke(options.Resolution);
    }

    void _on_v_sync_mode_item_selected(int index)
    {
        VSyncMode vsyncMode = (VSyncMode)index;
        WindowSetVsyncMode(vsyncMode);
        options.VSyncMode = vsyncMode;
        sliderMaxFPS.Editable = options.VSyncMode == VSyncMode.Disabled;
    }

    void _on_max_fps_value_changed(float value)
    {
        labelMaxFPSFeedback.Text = value == 0 ?
            "UNLIMITED" : value + "";

        options.MaxFPS = (int)value;
    }

    void _on_max_fps_drag_ended(bool valueChanged)
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

