using System.Collections.Generic;

namespace Template;

public class ConsoleHistory
{
    readonly Dictionary<int, string> inputHistory = [];
    int inputHistoryIndex;
    int inputHistoryNav;

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

    public bool NoHistory()
    {
        return inputHistory.Count == 0;
    }

    public string Get(int nav)
    {
        return inputHistory[nav];
    }
}

