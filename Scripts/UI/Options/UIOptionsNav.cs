namespace Template;

public partial class UIOptionsNav : Control
{
    private readonly Dictionary<string, Control> tabs = new();
    private readonly Dictionary<string, Button> buttons = new();

    public override void _Ready()
    {
        var content = GetParent().GetNode("Content");

        foreach (Control child in content.GetChildren())
            tabs.Add(child.Name, child);

        foreach (Button button in GetChildren())
        {
            button.FocusEntered += () => ShowTab(button.Name);
            button.Pressed += () => ShowTab(button.Name);

            buttons.Add(button.Name, button);
        }

        buttons[OptionsManager.CurrentOptionsTab].GrabFocus();

        HideAllTabs();
        ShowTab(OptionsManager.CurrentOptionsTab);
    }

    private void ShowTab(string tabName)
    {
        OptionsManager.CurrentOptionsTab = tabName;
        HideAllTabs();
        tabs[tabName].Show();
    }

    private void HideAllTabs() => tabs.Values.ForEach(x => x.Hide());
}
