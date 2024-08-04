namespace Template;

using System.IO;

public partial class Setup : Node
{
    string prevGameName = "";
    LineEdit lineEditGameName;
    Label gameNamePreview;
    PopupPanel popupPanel;
    Genre genre;
    
    enum Genre
    {
        Platformer2D,
        TopDown2D,
        FPS3D
    }

    public override void _Ready()
    {
        lineEditGameName = GetNode<LineEdit>("%GameName");
        gameNamePreview = GetNode<Label>("%GameNamePreview");
        popupPanel = GetNode<PopupPanel>("%PopupPanel");
        genre = (Genre)GetNode<OptionButton>("%Genre").Selected;
    }

    string FormatGameName(string name) => name.Trim().ToTitleCase().Replace(" ", "");

    void DisplayGameNamePreview(string inputName) =>
        gameNamePreview.Text = "The name will be " + FormatGameName(inputName);

    void _on_genre_item_selected(int index) => genre = (Genre)index;

    void _on_game_name_text_changed(string newText)
    {
        if (string.IsNullOrWhiteSpace(newText))
        {
            gameNamePreview.Text = "";
            return;
        }

        // Since this name is being used for the namespace its first character must not be
        // a number and every other character must be alphanumeric
        if (!IsAlphaNumeric(newText) || char.IsNumber(newText.Trim()[0]))
        {
            DisplayGameNamePreview(prevGameName);
            lineEditGameName.Text = prevGameName;
            lineEditGameName.CaretColumn = prevGameName.Length;
            return;
        }

        DisplayGameNamePreview(newText);
        prevGameName = newText;
    }

    void _on_no_pressed() => popupPanel.Hide();

    void _on_yes_pressed()
    {
        string gameName = FormatGameName(lineEditGameName.Text);

        if (string.IsNullOrWhiteSpace(gameName))
        {
            GD.Print("Please type a game name first!");
            return;
        }

        string path = ProjectSettings.GlobalizePath("res://");

        RenameProjectFiles(path, gameName);
        RenameAllNamespaces(path, gameName);
        MoveProjectFiles(path);

        GD.Print("The settings have been changed. Select \"Reload\" if prompted with it.");
        GD.Print("Please close the entire editor and re-open it.");

        GetTree().Quit();
    }

    void _on_apply_changes_pressed() => popupPanel.PopupCentered();

    void SetMainScene(string path, string scene)
    {
        string text = File.ReadAllText($"{path}project.godot");

        text = text.Replace(
            "run/main_scene=\"res://0 Setup/Setup.tscn\"",
           $"run/main_scene=\"res://Scenes/{scene}\"");

        File.WriteAllText($"{path}project.godot", text);
    }

    /// <summary>
    /// Deletes game assets that are not specific to the selected genre. For example if
    /// the 3D FPS genre was selected, the 2D Platformer and 2D Top Down folders would be
    /// deleted. The scene level_3D.tscn would be moved to res://Scenes and would be set as
    /// the new main project scene. In all cases the folder "0 Setup" is deleted.
    /// </summary>
    void MoveProjectFiles(string path)
    {
        Directory.Delete($"{path}0 Setup", true);

        string mainSceneName = "";

        const string FOLDER_NAME_FPS3D = "3D FPS";
        const string FOLDER_NAME_TOP_DOWN_2D = "2D Top Down";
        const string FOLDER_NAME_PLATFORMER_2D = "2D Platformer";

        switch (genre)
        {
            case Genre.Platformer2D:
                // Delete unneeded genre folders
                Directory.Delete($"{path}{FOLDER_NAME_TOP_DOWN_2D}", true);
                Directory.Delete($"{path}{FOLDER_NAME_FPS3D}", true);

                // Move main scene file
                mainSceneName = "level_2D_platformer.tscn";
                File.Move($"{path}{FOLDER_NAME_PLATFORMER_2D}/{mainSceneName}", $"{path}Scenes/{mainSceneName}");

                MoveFilesAndPreserveFolderStructure(path, FOLDER_NAME_PLATFORMER_2D);
                break;
            case Genre.TopDown2D:
                // Delete unneeded genre folders
                Directory.Delete($"{path}{FOLDER_NAME_PLATFORMER_2D}", true);
                Directory.Delete($"{path}{FOLDER_NAME_FPS3D}", true);

                // Move main scene file
                mainSceneName = "level_2D_top_down.tscn";
                File.Move($"{path}{FOLDER_NAME_TOP_DOWN_2D}/{mainSceneName}", $"{path}Scenes/{mainSceneName}");

                MoveFilesAndPreserveFolderStructure(path, FOLDER_NAME_TOP_DOWN_2D);
                break;
            case Genre.FPS3D:
                // Delete unneeded genre folders
                Directory.Delete($"{path}{FOLDER_NAME_PLATFORMER_2D}", true);
                Directory.Delete($"{path}{FOLDER_NAME_TOP_DOWN_2D}", true);

                // Move main scene file
                mainSceneName = "level_3D.tscn";
                File.Move($"{path}{FOLDER_NAME_FPS3D}/{mainSceneName}", $"{path}Scenes/{mainSceneName}");

                MoveFilesAndPreserveFolderStructure(path, FOLDER_NAME_FPS3D);
                break;
        }

        Debug.Assert(mainSceneName != "");

        SetMainScene(path, mainSceneName);

        DeleteDirectoryIfEmpty(path + FOLDER_NAME_TOP_DOWN_2D);
        DeleteDirectoryIfEmpty(path + FOLDER_NAME_PLATFORMER_2D);
        DeleteDirectoryIfEmpty(path + FOLDER_NAME_FPS3D);
    }

    void MoveFilesAndPreserveFolderStructure(string path, string folder)
    {
        // Move all scripts to res://Scripts
        TraverseDirectory(path + folder,
            fullPathFile =>
            {
                string newPath = fullPathFile.Replace(folder, "");

                new FileInfo(newPath).Directory.Create();

                try
                {
                    File.Move(fullPathFile, newPath);
                }
                catch (IOException)
                {
                    GD.Print($"Failed to move {fullPathFile.GetFile()}");
                }
            },
            directoryName => true);
    }

    void DeleteDirectoryIfEmpty(string path)
    {
        if (Directory.Exists(path))
        {
            GU.DeleteEmptyFolders(path);
            GU.DeleteEmptyFolder(path);
        }
    }

    /// <summary>
    /// Replaces all instances of the keyword "Template" with the new
    /// specified game name in several project files.
    /// </summary>
    void RenameProjectFiles(string path, string name)
    {
        // .csproj
        {
            string text = File.ReadAllText($"{path}Template.csproj");
            text = text.Replace("Template", name);
            File.Delete($"{path}Template.csproj");
            File.WriteAllText($"{path}{name}.csproj", text);
        }

        // .sln
        {
            string text = File.ReadAllText($"{path}Template.sln");
            text = text.Replace("Template", name);
            File.Delete($"{path}Template.sln");
            File.WriteAllText($"{path}{name}.sln", text);
        }

        // project.godot
        {
            string text = File.ReadAllText($"{path}project.godot");

            text = text.Replace(
                "project/assembly_name=\"Template\"", 
                $"project/assembly_name=\"{name}\"");

            text = text.Replace(
                "config/name=\"Template\"",
                $"config/name=\"{name}\""
                );

            File.WriteAllText($"{path}project.godot", text);
        }
    }

    /// <summary>
    /// Renames the default "Template" namespace to the new specified game
    /// name in all scripts.
    /// </summary>
    void RenameAllNamespaces(string path, string name)
    {
        TraverseDirectory(path, 
            fullPathFile =>
            {
                // Modify all scripts
                if (fullPathFile.EndsWith(".cs"))
                {
                    // Do not modify this script
                    if (!fullPathFile.EndsWith("Setup.cs"))
                    {
                        string text = File.ReadAllText(fullPathFile);
                        text = text.Replace("namespace Template", $"namespace {name}");
                        text = text.Replace("using Template", $"using {name}");
                        File.WriteAllText(fullPathFile, text);
                    }
                }
            }, 
            fullPathDir => !fullPathDir.EndsWith(".godot") && !fullPathDir.EndsWith("GodotUtils"));
    }

    /// <summary>
    /// <para>Traverse a directory given by 'path'.</para>
    /// <para>The action 'actionFile' has a full path to the file param.</para>
    /// <para>The action 'actionDirectory' has a full path to the directory param. 
    /// Returning true will recursively traverse this directory. Returning false 
    /// will not traverse the directory.</para>
    /// </summary>
    void TraverseDirectory(string path, Action<string> actionFile, Func<string, bool> actionDirectory)
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
                if (actionDirectory(fullPath))
                {
                    TraverseDirectory(fullPath, actionFile, actionDirectory);
                }
            }
            else
            {
                actionFile(fullPath);
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
