using System.IO;

namespace Template;

public partial class Setup : Node
{
    void _on_apply_changes_pressed()
    {
        string path = ProjectSettings.GlobalizePath("res://");
        RenameAllNamespaces(path, "Test");
    }

    void RenameAllNamespaces(string path, string name)
    {
        /*string path = ProjectSettings.GlobalizePath("res://0 Setup/Setup.cs");
        string text = File.ReadAllText(path);
        text = text.Replace("namespace Template", $"namespace {name}");
        File.Delete(path);
        File.WriteAllText(path, text);*/
        
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
            if (dir.CurrentIsDir())
            {
                if (fileName != ".godot" && fileName != "GodotUtils")
                {
                    GD.Print($@"{path}/{fileName}");
                    RenameAllNamespaces($@"{path}/{fileName}", name);
                }
            }
            else
            {
                if (fileName.EndsWith(".cs"))
                {

                }
            }

            fileName = dir.GetNext();
        }

        dir.ListDirEnd();
    }
}
