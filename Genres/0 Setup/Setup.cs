using Godot;
using GodotUtils;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;

namespace Template.Setup;

public partial class Setup : Node
{
    [Export] private LineEdit lineEditGameName;
    [Export] private OptionButton genreOptionBtn;
    [Export] private PopupPanel popupPanel;
    [Export] private RichTextLabel gameNamePreview;
    [Export] private RichTextLabel genreSelectedInfo;
    [Export] private CheckButton checkButtonDeleteSetupScene;
    [Export] private CheckButton checkButtonDeleteOtherGenres;
    [Export] private CheckButton checkButtonMoveProjectFiles;

    private string _prevGameName = string.Empty;
    private Genre _genre;

    private readonly Dictionary<Genre, string> _folderNames = new()
    {
        { Genre.None, "No Genre" },
        { Genre.Platformer2D, "2D Platformer" },
        { Genre.TopDown2D, "2D Top Down" },
        { Genre.FPS3D, "3D FPS" }
    };

    private enum Genre
    {
        None,
        Platformer2D,
        TopDown2D,
        FPS3D
    }

    public override void _Ready()
    {
        _genre = (Genre)genreOptionBtn.Selected;
        SetGenreSelectedInfo(_genre);
        DisplayGameNamePreview("Undefined");
    }
    
    private static void RestartEditor()
    {
        QuitEditor();
        StartEditor();
    }
    
    private static void StartEditor() 
    {
        OS.Execute(OS.GetExecutablePath(), ["--editor"]);
    }

    private static void QuitEditor()
    {
        string[] names = ["redot", "godot"];

        foreach (Process process in Process.GetProcesses())
        {
            foreach (string name in names)
            {
                if (process.ProcessName.Contains(name, StringComparison.OrdinalIgnoreCase))
                {
                    process.Kill();
                    return;
                }
            }
        }
    }

    private void SetGenreSelectedInfo(Genre genre)
    {
        string text = $"The {Highlight(_folderNames[genre])} genre has been selected. " +
              $"All other assets not specific to {Highlight(_folderNames[genre])} " +
              $"will be deleted.";

        genreSelectedInfo.Text = text;
    }

    private void DisplayGameNamePreview(string inputName)
    {
        string name = FormatGameName(inputName);

        string text = $"The name of the project will be {Highlight(name)}. " +
              $"The root namespace for all scripts will be {Highlight(name)}. " +
              $"Please ensure the name is in PascalFormat.";

        gameNamePreview.Text = text;
    }

    /// <summary> 
    /// Moves game assets specific to the selected genre to more accessible locations, 
    /// sets the main project scene, and removes any unnecessary files or folders. 
    /// For example, if the 3D FPS genre was chosen, this method would move the level_3D.tscn 
    /// file to res://Scenes, set it as the new main project scene and remove any other genre 
    /// assets (such as 2D Platformer or 2D Top Down). 
    /// </summary>
    private void MoveProjectFiles(string pathFrom, string pathTo)
    {
        // Gets the name of the main scene file based on the current genre
        string mainSceneName = GetMainSceneName(Path.Combine(pathFrom, _folderNames[_genre]));

        // Moves the main scene file from its original location to a new location
        File.Move(
            Path.Combine(pathFrom, _folderNames[_genre], mainSceneName), 
            Path.Combine(pathTo, "Scenes", mainSceneName));

        // Move all files relevant to this genre
        MoveFilesAndPreserveFolderStructure(Path.Combine(pathFrom, _folderNames[_genre]), Path.Combine("Genres", _folderNames[_genre]));

        if (checkButtonDeleteOtherGenres.ButtonPressed)
        {
            // If a directory is not associated with the current genre delete that folder
            foreach (KeyValuePair<Genre, string> folder in _folderNames)
            {
                if (folder.Key != _genre)
                {
                    Directory.Delete(Path.Combine(pathFrom, folder.Value), true);
                }
            }
        }

        // Sets the main scene for the project
        SetMainScene(pathTo, mainSceneName);

        // Deletes any empty directories that were previously moved
        DeleteDirectoryIfEmpty(pathFrom);
    }

    private void _on_genre_item_selected(int index)
    {
        _genre = (Genre)index;
        SetGenreSelectedInfo(_genre);
    }

    private void _on_game_name_text_changed(string newText)
    {
        if (string.IsNullOrWhiteSpace(newText))
        {
            return;
        }

        // Since this name is being used for the namespace its first character must not be
        // a number and every other character must be alphanumeric
        if (!IsAlphaNumericAndAllowSpaces(newText) || char.IsNumber(newText.Trim()[0]))
        {
            DisplayGameNamePreview(_prevGameName);
            lineEditGameName.Text = _prevGameName;
            lineEditGameName.CaretColumn = _prevGameName.Length;
            return;
        }

        DisplayGameNamePreview(newText);
        _prevGameName = newText;
    }

    private void _on_no_pressed()
    {
        popupPanel.Hide();
    }

    private void _on_yes_pressed()
    {
        string gameName = FormatGameName(lineEditGameName.Text);
        string path = ProjectSettings.GlobalizePath("res://");

        // The IO functions ran below will break if empty folders exist
        DirectoryUtils.DeleteEmptyDirectories(path);

        RenameProjectFiles(path, gameName);
        RenameAllNamespaces(path, gameName);

        if (checkButtonMoveProjectFiles.ButtonPressed)
        {
            MoveProjectFiles(Path.Combine(path, "Genres"), path);

            SceneFileUtils.FixBrokenDependencies();
        }

        if (checkButtonDeleteSetupScene.ButtonPressed)
        {
            // Delete the "0 Setup" directory
            Directory.Delete(Path.Combine(path, "Genres", "0 Setup"), true);
        }

        // Ensure all empty folders are deleted when finished
        DirectoryUtils.DeleteEmptyDirectories(path);

        GetTree().Quit();
        RestartEditor();
    }

    private void _on_apply_changes_pressed() 
    {
        string gameName = FormatGameName(lineEditGameName.Text);

        if (string.IsNullOrWhiteSpace(gameName))
        {
            GD.Print("Please type a game name first!");
            return;
        }

        popupPanel.PopupCentered();
    }

    private static string Highlight(string text)
    {
        return $"[wave amp=20.0 freq=2.0 connected=1][color=white]{text}[/color][/wave]";
    }

    private static string FormatGameName(string name)
    {
        return name.Trim().FirstCharToUpper().Replace(" ", "");
    }

    private static void SetMainScene(string path, string scene)
    {
        string text = File.ReadAllText(Path.Combine(path, "project.godot"));

        text = text.Replace(
            "run/main_scene=\"res://Genres/0 Setup/Setup.tscn\"",
           $"run/main_scene=\"res://Scenes/{scene}\"");

        File.WriteAllText(Path.Combine(path, "project.godot"), text);
    }

    private static string GetMainSceneName(string fullPath)
    {
        // Get all .tscn files in the directory
        string[] sceneFiles = Directory.GetFiles(fullPath, "*.tscn");

        // Check the number of .tscn files found
        if (sceneFiles.Length == 0)
        {
            throw new FileNotFoundException("No .tscn files found in the directory.");
        }
        else if (sceneFiles.Length > 1)
        {
            throw new InvalidOperationException("There can only be one main scene (.tscn file) in the root directory.");
        }

        // If there is exactly one .tscn file, return its name with the extension
        string sceneName = Path.GetFileName(sceneFiles[0]);

        return sceneName;
    }

    private static void MoveFilesAndPreserveFolderStructure(string path, string folder)
    {
        // Move all assets to res://
        DirectoryUtils.Traverse(path, MoveFile);

        void MoveFile(string fullFilePath)
        {
            string newPath = DirectoryUtils.RemovePathSegment(fullFilePath, folder);

            new FileInfo(newPath).Directory.Create();

            try
            {
                File.Move(fullFilePath, newPath);
            }
            catch (IOException)
            {
                GD.Print($"Failed to move {fullFilePath.GetFile()}");
            }
        }
    }

    private static void DeleteDirectoryIfEmpty(string path)
    {
        if (Directory.Exists(path))
        {
            DirectoryUtils.DeleteEmptyDirectories(path);
            DirectoryUtils.DeleteEmptyDirectory(path);
        }
    }

    /// <summary>
    /// Replaces all instances of the keyword "Template" with the new
    /// specified game name in several project files.
    /// </summary>
    private static void RenameProjectFiles(string path, string name)
    {
        // .csproj
        {
            string fullPath = Path.Combine(path, "Template.csproj");
            string text = File.ReadAllText(fullPath);
            text = text.Replace("<RootNamespace>Template</RootNamespace>", $"<RootNamespace>{name}</RootNamespace>");
            File.Delete(fullPath);
            File.WriteAllText(Path.Combine(path, name + ".csproj"), text);
        }

        // .sln
        {
            string fullPath = Path.Combine(path, "Template.sln");
            string text = File.ReadAllText(fullPath);
            text = text.Replace("Template", name);
            File.Delete(fullPath);
            File.WriteAllText(Path.Combine(path, name + ".sln"), text);
        }

        // project.godot
        {
            string fullPath = Path.Combine(path, "project.godot");
            string text = File.ReadAllText(fullPath);

            text = text.Replace(
                "project/assembly_name=\"Template\"", 
                $"project/assembly_name=\"{name}\"");

            text = text.Replace(
                "config/name=\"Template\"",
                $"config/name=\"{name}\""
                );

            File.WriteAllText(fullPath, text);
        }
    }

    /// <summary>
    /// Renames the default "Template" namespace to the new specified game
    /// name in all scripts.
    /// </summary>
    private static void RenameAllNamespaces(string path, string name)
    {
        DirectoryUtils.Traverse(path, RenameNamespaces);

        void RenameNamespaces(string fullFilePath)
        {
            // Ignore these directories
            switch (Path.GetDirectoryName(fullFilePath))
            {
                case ".godot":
                case "GodotUtils":
                case "addons":
                    return;
            }

            // Modify all scripts
            if (fullFilePath.EndsWith(".cs"))
            {
                // Do not modify this script
                if (!fullFilePath.EndsWith("Setup.cs"))
                {
                    string text = File.ReadAllText(fullFilePath);
                    text = text.Replace("namespace Template", $"namespace {name}");
                    text = text.Replace("using Template", $"using {name}");
                    text = text.Replace("Template.", $"{name}.");
                    File.WriteAllText(fullFilePath, text);
                }
            }
        }
    }

    [GeneratedRegex(@"^[a-zA-Z0-9\s,]*$")]
    private static partial Regex AlphaNumericAndSpacesRegex();

    private static bool IsAlphaNumericAndAllowSpaces(string str)
    {
        return AlphaNumericAndSpacesRegex().IsMatch(str);
    }
}
