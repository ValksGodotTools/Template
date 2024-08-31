namespace GodotUtils;

using System.IO;

public static class GSceneFileUtils
{
    /// <summary>
    /// Loads a scene from res://Scenes/Prefabs and instantiates it
    /// </summary>
    public static T LoadPrefab<T>(string prefab) where T : Node =>
        (T)GD.Load<PackedScene>($"res://Scenes/Prefabs/{prefab}.tscn").Instantiate();

    public static void FixBrokenDependencies()
    {
        GDirectories.Traverse(ProjectSettings.GlobalizePath("res://"), FixBrokenResourcePaths);
    }

    private static void FixBrokenResourcePaths(string fullFilePath)
    {
        if (fullFilePath.EndsWith(".tscn"))
        {
            FixBrokenResourcePath(fullFilePath, new Regex("path=\"(?<path>.+)\" "));
        }
        else if (fullFilePath.EndsWith(".glb.import"))
        {
            FixBrokenResourcePath(fullFilePath, new Regex("\"save_to_file/path\": \"(?<path>.+)\""));
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
                string newResourcePathGlobal = GDirectories.FindFile(ProjectSettings.GlobalizePath("res://"), oldResourcePath.GetFile());

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
}
