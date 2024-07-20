using System.IO;

namespace Template;

public partial class Setup : Node
{
    string prevGameName = "";
    LineEdit lineEditGameName;

    public override void _Ready()
    {
        lineEditGameName = GetNode<LineEdit>("%GameName");
    }

    void _on_game_name_text_changed(string newText)
    {
        if (string.IsNullOrWhiteSpace(newText))
            return;

        // Since this name is being used for the namespace its first character must not be
        // a number and every other character must be alphanumeric
        if (!IsAlphaNumeric(newText) || char.IsNumber(newText.Trim()[0]))
        {
            lineEditGameName.Text = prevGameName;
            lineEditGameName.CaretColumn = prevGameName.Length;
            return;
        }

        prevGameName = newText;
    }

    void _on_apply_changes_pressed()
    {
        string gameName = lineEditGameName.Text.Trim().ToTitleCase().Replace(" ", "");

        if (string.IsNullOrWhiteSpace(gameName))
        {
            GD.Print("Please type in a game name first!");
            return;
        }

        //string path = ProjectSettings.GlobalizePath("res://");
        //RenameAllNamespaces(path, gameName);
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

    bool IsAlphaNumeric(string str)
    {
        Regex rg = new Regex(@"^[a-zA-Z0-9\s,]*$");
        return rg.IsMatch(str);
    }
}
