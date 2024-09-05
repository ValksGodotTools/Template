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

public partial class UIOptionsNav : Control
{
    [Export] OptionsManager optionsManager;

    readonly Dictionary<string, Control> tabs = new();
    readonly Dictionary<string, Button> buttons = new();

    public override void _Ready()
    {
        Node content = GetParent().GetNode("Content");

        foreach (Control child in content.GetChildren())
            tabs.Add(child.Name, child);

        foreach (Button button in GetChildren())
        {
            button.FocusEntered += () => ShowTab(button.Name);
            button.Pressed += () => ShowTab(button.Name);

            buttons.Add(button.Name, button);
        }

        buttons[optionsManager.CurrentOptionsTab].GrabFocus();

        HideAllTabs();
        ShowTab(optionsManager.CurrentOptionsTab);
    }

    void ShowTab(string tabName)
    {
        optionsManager.CurrentOptionsTab = tabName;
        HideAllTabs();
        tabs[tabName].Show();
    }

    void HideAllTabs() => tabs.Values.ForEach(x => x.Hide());
}

