using Godot;
using System.IO;
using System.Text.RegularExpressions;

namespace GodotUtils;

public static partial class SceneFileUtils
{
    public static void FixBrokenDependencies()
    {
        DirectoryUtils.Traverse("res://", FixBrokenResourcePaths);
    }

    private static void FixBrokenResourcePaths(string fullFilePath)
    {
        if (fullFilePath.EndsWith(".tscn"))
        {
            FixBrokenResourcePath(fullFilePath, TSCNPath());
        }
        else if (fullFilePath.EndsWith(".glb.import"))
        {
            FixBrokenResourcePath(fullFilePath, GLBPath());
        }
    }

    private static void FixBrokenResourcePath(string fullFilePath, Regex regex)
    {
        string text = File.ReadAllText(fullFilePath);

        foreach (Match match in regex.Matches(text))
        {
            string oldResourcePath = match.Groups["path"].Value;

            if (!Godot.FileAccess.FileExists(oldResourcePath))
            {
                string newResourcePathGlobal = DirectoryUtils.FindFile("res://", oldResourcePath.GetFile());

                if (!string.IsNullOrWhiteSpace(newResourcePathGlobal))
                {
                    string newResourcePathLocal = ProjectSettings.LocalizePath(newResourcePathGlobal);

                    text = text.Replace(oldResourcePath, newResourcePathLocal);
                }
                else
                {
                    GD.Print($"Failed to fix a resource path for the scene '{fullFilePath.GetFile()}'. The resource '{oldResourcePath.GetFile()}' could not be found in the project.");
                }
            }
        }

        File.WriteAllText(fullFilePath, text);
    }

    [GeneratedRegex("path=\"(?<path>.+)\" ")]
    private static partial Regex TSCNPath();

    [GeneratedRegex("\"save_to_file/path\": \"(?<path>.+)\"")]
    private static partial Regex GLBPath();
}

