using CSharpUtils;
using Godot;
using GodotUtils;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Template;

using System.IO;

public partial class Setup : Node
{
    [Export] LineEdit lineEditGameName;
    [Export] OptionButton genreOptionBtn;
    [Export] PopupPanel popupPanel;
    [Export] RichTextLabel gameNamePreview;
    [Export] RichTextLabel genreSelectedInfo;
    [Export] CheckButton checkButtonDeleteSetupScene;
    [Export] CheckButton checkButtonDeleteOtherGenres;
    [Export] CheckButton checkButtonMoveProjectFiles;

    string prevGameName = "";
    Genre genre;

    readonly Dictionary<Genre, string> folderNames = new()
    {
        { Genre.None, "No Genre" },
        { Genre.Platformer2D, "2D Platformer" },
        { Genre.TopDown2D, "2D Top Down" },
        { Genre.FPS3D, "3D FPS" }
    };

    enum Genre
    {
        None,
        Platformer2D,
        TopDown2D,
        FPS3D
    }

    public override void _Ready()
    {
        genre = (Genre)genreOptionBtn.Selected;
        SetGenreSelectedInfo(genre);
        DisplayGameNamePreview("Undefined");
    }

    private void SetGenreSelectedInfo(Genre genre)
    {
        string text = $"The {Highlight(folderNames[genre])} genre has been selected. The main scene will be set to " +
            $"{Highlight($"res://Scenes/{GetMainSceneName(genre)}")}. All other assets not specific to {Highlight(folderNames[genre])} " +
            $"will be deleted.";

        genreSelectedInfo.Text = text;
    }

    private void DisplayGameNamePreview(string inputName)
    {
        string name = FormatGameName(inputName);
        
        string text = $"The name of the project will be {Highlight(name)}. The root namespace for all scripts will " +
            $"be {Highlight(name)}. Please ensure the name is in PascalFormat.";
        
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
        string mainSceneName = GetMainSceneName(genre);

        // Moves the main scene file from its original location to a new location
        File.Move($"{pathFrom}{folderNames[genre]}/{mainSceneName}", $"{pathTo}Scenes/{mainSceneName}");

        // Move all files relevant to this genre
        MoveFilesAndPreserveFolderStructure(pathFrom, "Genres//" + folderNames[genre]);

        if (checkButtonDeleteOtherGenres.ButtonPressed)
        {
            // If a directory is not associated with the current genre delete that folder
            foreach (KeyValuePair<Genre, string> folder in folderNames)
            {
                if (folder.Key != genre)
                    Directory.Delete($"{pathFrom}{folder.Value}", true);
            }
        }

        // Sets the main scene for the project
        SetMainScene(pathTo, mainSceneName);

        // Deletes any empty directories that were previously moved
        DeleteDirectoryIfEmpty(pathFrom);
    }

    private void _on_genre_item_selected(int index)
    {
        genre = (Genre)index;
        SetGenreSelectedInfo(genre);
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
            DisplayGameNamePreview(prevGameName);
            lineEditGameName.Text = prevGameName;
            lineEditGameName.CaretColumn = prevGameName.Length;
            return;
        }

        DisplayGameNamePreview(newText);
        prevGameName = newText;
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
        GDirectories.DeleteEmptyDirectories(path);

        RenameProjectFiles(path, gameName);
        RenameAllNamespaces(path, gameName);

        if (checkButtonMoveProjectFiles.ButtonPressed)
        {
            MoveProjectFiles(path + @"Genres/", path);

            GSceneFileUtils.FixBrokenDependencies();
        }

        if (checkButtonDeleteSetupScene.ButtonPressed)
        {
            // Delete the "0 Setup" directory
            Directory.Delete(path + @"Genres/0 Setup", true);
        }

        // Ensure all empty folders are deleted when finished
        GDirectories.DeleteEmptyDirectories(path);

        GetTree().Quit();
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
        string text = File.ReadAllText($"{path}project.godot");

        text = text.Replace(
            "run/main_scene=\"res://Genres/0 Setup/setup.tscn\"",
           $"run/main_scene=\"res://Scenes/{scene}\"");

        File.WriteAllText($"{path}project.godot", text);
    }

    private static string GetMainSceneName(Genre genre)
    {
        return genre switch
        {
            Genre.None => "main",
            Genre.Platformer2D => "level_2D_platformer",
            Genre.TopDown2D => "level_2D_top_down",
            Genre.FPS3D => "level_3D",
            _ => throw new NotImplementedException()
        } + ".tscn";
    }

    private static void MoveFilesAndPreserveFolderStructure(string path, string folder)
    {
        // Move all scripts to res://Scripts
        TraverseDirectory(path,
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

    private static void DeleteDirectoryIfEmpty(string path)
    {
        if (Directory.Exists(path))
        {
            GDirectories.DeleteEmptyDirectories(path);
            GDirectories.DeleteEmptyDirectory(path);
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
    private static void RenameAllNamespaces(string path, string name)
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
    private static void TraverseDirectory(string path, Action<string> actionFile, Func<string, bool> actionDirectory)
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
            string fullPath = $"{path}/{fileName}";

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

    private static bool IsAlphaNumericAndAllowSpaces(string str)
    {
        Regex rg = new(@"^[a-zA-Z0-9\s,]*$");
        return rg.IsMatch(str);
    }
}

