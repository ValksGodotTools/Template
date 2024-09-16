using Godot;
using GodotUtils;
using System;

using static Godot.DisplayServer;

namespace Template;

public partial class UIOptionsDisplay : Control
{
    public event Action<int> OnResolutionChanged;

    [Export] OptionsManager optionsManager;

    ResourceOptions _options;

    // Max FPS
    HSlider _sliderMaxFPS;
    Label _labelMaxFPSFeedback;

    // Window Size
    LineEdit _resX, _resY;

    int _prevNumX, _prevNumY;
    int _min_resolution = 36;

    public override void _Ready()
    {
        _options = optionsManager.Options;
        SetupMaxFPS();
        SetupWindowSize();
        SetupWindowMode();
        SetupResolution();
        SetupVSyncMode();
    }

    void SetupMaxFPS()
    {
        _labelMaxFPSFeedback = GetNode<Label>("%MaxFPSFeedback");
        _labelMaxFPSFeedback.Text = _options.MaxFPS == 0 ?
            "UNLIMITED" : _options.MaxFPS + "";

        _sliderMaxFPS = GetNode<HSlider>("%MaxFPS");
        _sliderMaxFPS.Value = _options.MaxFPS;
        _sliderMaxFPS.Editable = _options.VSyncMode == VSyncMode.Disabled;
    }

    void SetupWindowSize()
    {
        _resX = GetNode<LineEdit>("%WindowWidth");
        _resY = GetNode<LineEdit>("%WindowHeight");

        Vector2I winSize = DisplayServer.WindowGetSize();

        _prevNumX = winSize.X;
        _prevNumY = winSize.Y;

        _resX.Text = winSize.X + "";
        _resY.Text = winSize.Y + "";
    }

    void SetupWindowMode()
    {
        OptionButton optionBtnWindowMode = GetNode<OptionButton>("%WindowMode");
        optionBtnWindowMode.Select((int)_options.WindowMode);

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
        GetNode<HSlider>("%Resolution").Value = 1 + _min_resolution - _options.Resolution;
    }

    void SetupVSyncMode()
    {
        GetNode<OptionButton>("%VSyncMode").Select((int)_options.VSyncMode);
    }

    void ApplyWindowSize()
    {
        DisplayServer.WindowSetSize(new Vector2I(_prevNumX, _prevNumY));

        // Center window
        Vector2I winSize = DisplayServer.WindowGetSize();
        DisplayServer.WindowSetPosition(DisplayServer.ScreenGetSize() / 2 - winSize / 2);

        _options.WindowSize = winSize;
    }

    void _on_window_mode_item_selected(int index)
    {
        switch ((WindowMode)index)
        {
            case WindowMode.Windowed:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                _options.WindowMode = WindowMode.Windowed;
                break;
            case WindowMode.Borderless:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
                _options.WindowMode = WindowMode.Borderless;
                break;
            case WindowMode.Fullscreen:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
                _options.WindowMode = WindowMode.Fullscreen;
                break;
        }

        // Update UIWindowSize element on window mode change
        Vector2I winSize = DisplayServer.WindowGetSize();

        _resX.Text = winSize.X + "";
        _resY.Text = winSize.Y + "";
        _prevNumX = winSize.X;
        _prevNumY = winSize.Y;

        _options.WindowSize = winSize;
    }

    void _on_window_width_text_changed(string text)
    {
        text.ValidateNumber(_resX, 0, ScreenGetSize().X, ref _prevNumX);
    }

    void _on_window_height_text_changed(string text)
    {
        text.ValidateNumber(_resY, 0, ScreenGetSize().Y, ref _prevNumY);
    }

    void _on_window_width_text_submitted(string t)
    {
        ApplyWindowSize();
    }

    void _on_window_height_text_submitted(string t)
    {
        ApplyWindowSize();
    }

    void _on_window_size_apply_pressed()
    {
        ApplyWindowSize();
    }

    void _on_resolution_value_changed(float value)
    {
        _options.Resolution = _min_resolution - (int)value + 1;
        OnResolutionChanged?.Invoke(_options.Resolution);
    }

    void _on_v_sync_mode_item_selected(int index)
    {
        VSyncMode vsyncMode = (VSyncMode)index;
        WindowSetVsyncMode(vsyncMode);
        _options.VSyncMode = vsyncMode;
        _sliderMaxFPS.Editable = _options.VSyncMode == VSyncMode.Disabled;
    }

    void _on_max_fps_value_changed(float value)
    {
        _labelMaxFPSFeedback.Text = value == 0 ?
            "UNLIMITED" : value + "";

        _options.MaxFPS = (int)value;
    }

    void _on_max_fps_drag_ended(bool valueChanged)
    {
        if (!valueChanged)
            return;

        Engine.MaxFps = _options.MaxFPS;
    }
}

public enum WindowMode
{
    Windowed,
    Borderless,
    Fullscreen
}

