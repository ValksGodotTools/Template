namespace GodotUtils;

public class ConsoleHistory
{
    private Dictionary<int, string> InputHistory      { get; } = new();
    private int                     InputHistoryIndex { get; set; }
    private int                     InputHistoryNav   { get; set; }

    /// <summary>
    /// Add text to history
    /// </summary>
    public void Add(string text)
    {
        InputHistory.Add(InputHistoryIndex++, text);
        InputHistoryNav = InputHistoryIndex;
    }

    /// <summary>
    /// Move up one in history
    /// </summary>
    public string MoveUpOne()
    {
        InputHistoryNav--;

        if (!InputHistory.ContainsKey(InputHistoryNav))
            InputHistoryNav++;

        return Get(InputHistoryNav);
    }

    /// <summary>
    /// Move down one in history
    /// </summary>
    public string MoveDownOne()
    {
        InputHistoryNav++;

        if (!InputHistory.ContainsKey(InputHistoryNav))
            InputHistoryNav--;

        return Get(InputHistoryNav);
    }

    public bool NoHistory() => InputHistory.Count == 0;

    public string Get(int nav) => InputHistory[nav];
}
