namespace Template;

public partial class UIOptionsNav : Control
{
    private readonly Dictionary<string, Control> tabs = new();

    public override void _Ready()
    {
        var content = GetParent().GetNode("Content");

        foreach (Control child in content.GetChildren())
            tabs.Add(child.Name, child);

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

    private void _on_general_pressed() => ShowTab("General");
    private void _on_display_pressed() => ShowTab("Display");
    private void _on_audio_pressed() => ShowTab("Audio");
    private void _on_input_pressed() => ShowTab("Input");
}
