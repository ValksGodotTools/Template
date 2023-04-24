namespace Template;

public partial class UIOptionsNav : Control
{
    private Dictionary<string, Control> Tabs { get; } = new();

    public override void _Ready()
    {
        var content = GetParent().GetNode("Content");

        foreach (Control child in content.GetChildren())
            Tabs.Add(child.Name, child);

        HideAllTabs();
        ShowTab("General");
    }

    private void ShowTab(string tabName)
    {
        HideAllTabs();
        Tabs[tabName].Show();
    }

    private void HideAllTabs() => Tabs.Values.ForEach(x => x.Hide());

    private void _on_general_pressed() => ShowTab("General");
    private void _on_display_pressed() => ShowTab("Display");
    private void _on_audio_pressed() => ShowTab("Audio");
    private void _on_input_pressed() => ShowTab("Input");
}
