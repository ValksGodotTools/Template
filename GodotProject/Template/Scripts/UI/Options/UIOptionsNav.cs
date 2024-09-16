using CSharpUtils;
using Godot;
using System.Collections.Generic;

namespace Template;

public partial class UIOptionsNav : Control
{
    [Export] OptionsManager optionsManager;

    readonly Dictionary<string, Control> _tabs = [];
    readonly Dictionary<string, Button> _buttons = [];

    public override void _Ready()
    {
        Node content = GetParent().GetNode("Content");

        foreach (Control child in content.GetChildren())
            _tabs.Add(child.Name, child);

        foreach (Button button in GetChildren())
        {
            button.FocusEntered += () => ShowTab(button.Name);
            button.Pressed += () => ShowTab(button.Name);

            _buttons.Add(button.Name, button);
        }

        _buttons[optionsManager.CurrentOptionsTab].GrabFocus();

        HideAllTabs();
        ShowTab(optionsManager.CurrentOptionsTab);
    }

    void ShowTab(string tabName)
    {
        optionsManager.CurrentOptionsTab = tabName;
        HideAllTabs();
        _tabs[tabName].Show();
    }

    void HideAllTabs()
    {
        _tabs.Values.ForEach(x => x.Hide());
    }
}

