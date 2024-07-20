using System.IO;

namespace Template;

public partial class Setup : Node
{
    void _on_apply_changes_pressed()
    {
        string path = ProjectSettings.GlobalizePath("res://");
        RenameAllNamespaces(path, "Banana");
    }

    void RenameAllNamespaces(string path, string name)
    {
        using DirAccess dir = DirAccess.Open(path);

        if (dir == null)
        {
            GD.Print($"An error occured when trying to access the path '{path}'");
            return;
        }

        dir.ListDirBegin();

        string fileName = dir.GetNext();

        while (fileName != "")
        {
            string fullPath = $@"{path}/{fileName}";

            if (dir.CurrentIsDir())
            {
                if (fileName != ".godot" && fileName != "GodotUtils")
                {
                    RenameAllNamespaces(fullPath, name);
                }
            }
            else
            {
                if (fileName.EndsWith(".cs"))
                {
                    string text = File.ReadAllText(fullPath);
                    text = text.Replace("namespace Template", $"namespace {name}");
                    File.WriteAllText(fullPath, text);
                }
            }

            fileName = dir.GetNext();
        }

        dir.ListDirEnd();
    }
}
