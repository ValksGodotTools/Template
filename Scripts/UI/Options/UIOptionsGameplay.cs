namespace Template;

public partial class UIOptionsGameplay : Control
{
    private ResourceOptions options;

    // Difficulty
    private OptionButton optionBtnDifficulty;

    public override void _Ready()
    {
        options = OptionsManager.Options;
        SetupDifficulty();
    }

    private void SetupDifficulty()
    {
        optionBtnDifficulty = GetNode<OptionButton>("Difficulty/Difficulty");
        optionBtnDifficulty.Select((int)options.Difficulty);
    }

    private void _on_difficulty_item_selected(int index)
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
