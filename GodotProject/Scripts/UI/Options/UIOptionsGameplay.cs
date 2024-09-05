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

public partial class UIOptionsGameplay : Control
{
    public event Action<float> OnMouseSensitivityChanged;

    [Export] OptionsManager optionsManager;

    ResourceOptions options;

    public override void _Ready()
    {
        options = optionsManager.Options;
        SetupDifficulty();
        SetupMouseSensitivity();
    }

    void SetupDifficulty()
    {
        GetNode<OptionButton>("%Difficulty").Select((int)options.Difficulty);
    }

    void SetupMouseSensitivity()
    {
        GetNode<HSlider>("%Sensitivity").Value = options.MouseSensitivity;
    }

    void _on_difficulty_item_selected(int index)
    {
        // todo: update the difficulty in realtime

        options.Difficulty = (Difficulty)index;
    }

    void _on_sensitivity_value_changed(float value)
    {
        options.MouseSensitivity = value;
        OnMouseSensitivityChanged?.Invoke(value);
    }
}

public enum Difficulty
{
    Easy,
    Normal,
    Hard
}

