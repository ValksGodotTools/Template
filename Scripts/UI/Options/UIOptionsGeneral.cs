namespace Template;

public partial class UIOptionsGeneral : Control
{
    [Export] OptionsManager optionsManager;

    ResourceOptions options;

    public override void _Ready()
    {
        options = optionsManager.Options;
        SetupLanguage();
    }

    void SetupLanguage()
    {
        var optionButtonLanguage = GetNode<OptionButton>("%Language");
        optionButtonLanguage.Select((int)options.Language);
    }

    void _on_language_item_selected(int index)
    {
        string locale = ((Language)index).ToString().Substring(0, 2).ToLower();

        TranslationServer.SetLocale(locale);

        options.Language = (Language)index;
    }
}

public enum Language
{
    English,
    French,
    Japanese
}
