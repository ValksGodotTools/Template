using Godot;
using System;

namespace Template;

public partial class UIOptionsGameplay : Control
{
    public event Action<float> OnMouseSensitivityChanged;

    [Export] OptionsManager optionsManager;

    ResourceOptions _options;

    public override void _Ready()
    {
        _options = optionsManager.Options;
        SetupDifficulty();
        SetupMouseSensitivity();
    }

    void SetupDifficulty()
    {
        GetNode<OptionButton>("%Difficulty").Select((int)_options.Difficulty);
    }

    void SetupMouseSensitivity()
    {
        GetNode<HSlider>("%Sensitivity").Value = _options.MouseSensitivity;
    }

    void _on_difficulty_item_selected(int index)
    {
        // todo: update the difficulty in realtime

        _options.Difficulty = (Difficulty)index;
    }

    void _on_sensitivity_value_changed(float value)
    {
        _options.MouseSensitivity = value;
        OnMouseSensitivityChanged?.Invoke(value);
    }
}

public enum Difficulty
{
    Easy,
    Normal,
    Hard
}

