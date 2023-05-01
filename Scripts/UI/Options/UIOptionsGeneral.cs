namespace Template;

public partial class UIOptionsGeneral : Control
{
    private ResourceOptions options;

    public override void _Ready()
    {
        options = OptionsManager.Options;
        SetupLanguage();
    }

    private void SetupLanguage()
    {
        var optionButtonLanguage = GetNode<OptionButton>("Language/Language");
        optionButtonLanguage.Select((int)options.Language);
    }

    private void _on_language_item_selected(int index)
    {
        var locale = ((Language)index).ToString().Substring(0, 2).ToLower();

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
