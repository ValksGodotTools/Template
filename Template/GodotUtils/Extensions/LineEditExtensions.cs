using Godot;
using System.Collections.Generic;
using System;

namespace GodotUtils;

public static class LineEditExtensions
{
    private static readonly Dictionary<ulong, string> prevTexts = [];

    /// <summary>
    /// <para>
    /// Prevents certain characters from being inputted onto the LineEdit.
    /// The following example makes it so only alphanumeric characters are allowed
    /// to be inputted.
    /// </para>
    /// 
    /// <code>
    /// LineEdit.TextChanged += text =>
    /// {
    ///     string username = LineEdit.Filter(text => text.IsAlphaNumeric());
    ///     GD.Print(username);
    /// };
    /// </code>
    /// </summary>
    public static string Filter(this LineEdit lineEdit, Func<string, bool> filter)
    {
        ulong id = lineEdit.GetInstanceId();

        if (!filter(lineEdit.Text))
        {
            lineEdit.Text = prevTexts[id];
            lineEdit.CaretColumn = prevTexts[id].Length;
            return prevTexts.TryGetValue(id, out string value) ? value : "";
        }

        prevTexts[id] = lineEdit.Text;

        return lineEdit.Text;
    }

    public static void ValidateNumber(this string value, LineEdit input, int min, int max, ref int prevNum)
    {
        // do NOT use text.Clear() as it will trigger _on_NumAttempts_text_changed and cause infinite loop -> stack overflow
        if (string.IsNullOrEmpty(value))
        {
            prevNum = 0;
            EditInputText(input, "");
            return;
        }

        if (!int.TryParse(value.Trim(), out int num))
        {
            EditInputText(input, $"{prevNum}");
            return;
        }

        if (value.Length > max.ToString().Length && num <= max)
        {
            string spliced = value.Remove(value.Length - 1);
            prevNum = int.Parse(spliced);
            EditInputText(input, spliced);
            return;
        }

        if (num < min)
        {
            num = min;
            EditInputText(input, $"{min}");
        }

        if (num > max)
        {
            num = max;
            EditInputText(input, $"{max}");
        }

        prevNum = num;
    }

    private static void EditInputText(LineEdit input, string text)
    {
        input.Text = text;
        input.CaretColumn = input.Text.Length;
    }
}

