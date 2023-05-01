namespace GodotUtils;

public class ConsoleHistory
{
    private readonly Dictionary<int, string> inputHistory = new();
    private int inputHistoryIndex;
    private int inputHistoryNav;

    /// <summary>
    /// Add text to history
    /// </summary>
    public void Add(string text)
    {
        inputHistory.Add(inputHistoryIndex++, text);
        inputHistoryNav = inputHistoryIndex;
    }

    /// <summary>
    /// Move up one in history
    /// </summary>
    public string MoveUpOne()
    {
        inputHistoryNav--;

        if (!inputHistory.ContainsKey(inputHistoryNav))
            inputHistoryNav++;

        return Get(inputHistoryNav);
    }

    /// <summary>
    /// Move down one in history
    /// </summary>
    public string MoveDownOne()
    {
        inputHistoryNav++;

        if (!inputHistory.ContainsKey(inputHistoryNav))
            inputHistoryNav--;

        return Get(inputHistoryNav);
    }

    public bool NoHistory() => inputHistory.Count == 0;

    public string Get(int nav) => inputHistory[nav];
}
