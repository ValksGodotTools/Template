namespace GodotUtils;

using Godot;
using System;
using System.Collections.Generic;

public static class ExtensionsTextEdit
{
    static readonly Dictionary<ulong, string> prevTexts = new();

    public static string Filter(this TextEdit textEdit, Func<string, bool> filter)
    {
        string text = textEdit.Text;
        ulong id = textEdit.GetInstanceId();

        if (string.IsNullOrWhiteSpace(text))
            return prevTexts.ContainsKey(id) ? prevTexts[id] : null;

        if (!filter(text))
        {
            if (!prevTexts.ContainsKey(id))
            {
                textEdit.ChangeTextEditText("");
                return null;
            }

            textEdit.ChangeTextEditText(prevTexts[id]);
            return prevTexts[id];
        }

        prevTexts[id] = text;
        return text;
    }
    static void ChangeTextEditText(this TextEdit textEdit, string text)
    {
        textEdit.Text = text;
        //textEdit.CaretColumn = text.Length;
    }
}
