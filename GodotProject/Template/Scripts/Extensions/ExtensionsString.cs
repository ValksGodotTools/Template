namespace GodotUtils;

using Godot;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

public static class ExtensionsString
{
    /// <summary>
    /// Checks if a string is a valid IP address. Entering any kind of IP like 127.0.0.1
    /// or localhost are valid. This function does not check for domains like play.apex.ca
    /// </summary>
    public static bool IsAddress(this string v) =>
        v != null && (Regex.IsMatch(v, @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}") || v.Contains("localhost"));

    /// <summary>
    /// This will transform for example "helloWorld" to "hello World"
    /// </summary>
    public static string AddSpaceBeforeEachCapital(this string v) =>
        string.Concat(v.Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');

    /// <summary>
    /// Returns true if the string contains only letters or digits
    /// </summary>
    public static bool IsAlphaNumeric(this string v) => v.All(char.IsLetterOrDigit);

    /// <summary>
    /// Returns true if the string contains only letters
    /// </summary>
    public static bool IsAlphaOnly(this string v) => v.All(char.IsLetter);

    /// <summary>
    /// Returns true if the string contains only digits
    /// </summary>
    public static bool IsNumericOnly(this string v) => v.All(char.IsDigit);

    /// <summary>
    /// This will transform for example "hello world" to "Hello World"
    /// </summary>
    public static string ToTitleCase(this string v) =>
        CultureInfo.CurrentCulture.TextInfo.ToTitleCase(v.ToLower());

    /// <summary>
    /// If max length is set to 2 then all words smaller than or equal to 2
    /// characters will be transformed to uppercase. For example "The ip is 127.0.0.1"
    /// would be transformed to "The IP is 127.0.0.1"
    /// </summary>
    public static string SmallWordsToUpper(this string v, int maxLength = 2, Func<string, bool> filter = null)
    {
        string[] words = v.Split(' ');

        for (int i = 0; i < words.Length; i++)
            if (words[i].Length <= maxLength && (filter == null || filter(words[i])))
                words[i] = words[i].ToUpper();

        return string.Join(" ", words);
    }

    /// <summary>
    /// Strips all BBCode from a given string. Note that this means you can no
    /// longer use square brackets '[]' for stylish purposes.
    /// </summary>
    public static string StripBBCode(this string source)
    {
        RegEx regex = new RegEx();
        regex.Compile("\\[.+?\\]");
        return regex.Sub(source, replacement: "", all: true);
    }

    /// <summary>
    /// Converts the first character in a string to be Uppercase
    /// </summary>
    public static string FirstCharToUpper(this string input)
    {
        if (input is null)
            throw new ArgumentNullException(nameof(input));

        if (input is "")
            return input;

        return string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1));
    }
}
