using Godot;

namespace Template;

public partial class UIOptionsGeneral : Control
{
    [Export] private OptionsManager optionsManager;
    private ResourceOptions _options;

    public override void _Ready()
    {
        _options = optionsManager.Options;
        SetupLanguage();
    }

    private void SetupLanguage()
    {
        OptionButton optionButtonLanguage = GetNode<OptionButton>("%Language");
        optionButtonLanguage.Select((int)_options.Language);
    }

    private void _on_language_item_selected(int index)
    {
        string locale = ((Language)index).ToString().Substring(0, 2).ToLower();

        TranslationServer.SetLocale(locale);

        _options.Language = (Language)index;
    }
}

public enum Language
{
    English,
    French,
    Japanese
}

