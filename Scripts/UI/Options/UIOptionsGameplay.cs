namespace Template;

public partial class UIOptionsGameplay : Control
{
    ResourceOptions options;

    // Difficulty
    OptionButton optionBtnDifficulty;

    public override void _Ready()
    {
        options = OptionsManager.Options;
        SetupDifficulty();
    }

    void SetupDifficulty()
    {
        optionBtnDifficulty = GetNode<OptionButton>("%Difficulty");
        optionBtnDifficulty.Select((int)options.Difficulty);
    }

    void _on_difficulty_item_selected(int index)
    {
        // todo: update the difficulty in realtime

        options.Difficulty = (Difficulty)index;
    }
}

public enum Difficulty
{
    Easy,
    Normal,
    Hard
}
