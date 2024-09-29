using Godot;
using System.IO;
using System;

namespace GodotUtils;

public static class DirectoryUtils
{
    /// <summary>
    /// Recursively traverses all directories and performs a action on each file path. 
    /// 
    /// <code>
    /// GDirectories.Traverse("res://", fullFilePath => GD.Print(fullFilePath))
    /// </code>
    /// </summary>
    public static void Traverse(string directory, Action<string> actionFullFilePath)
    {
        directory = NormalizePath(ProjectSettings.GlobalizePath(directory));

        using DirAccess dir = DirAccess.Open(directory);

        dir.ListDirBegin();

        string nextFileName;

        while ((nextFileName = dir.GetNext()) != string.Empty)
        {
            string fullFilePath = Path.Combine(directory, nextFileName);

            if (dir.CurrentIsDir())
            {
                if (!nextFileName.StartsWith('.'))
                {
                    Traverse(fullFilePath, actionFullFilePath);
                }
            }
            else
            {
                actionFullFilePath(fullFilePath);
            }
        }

        dir.ListDirEnd();
    }

    /// <summary>
    /// Recursively searches for the file name and if found returns the full file path to
    /// that file.
    /// 
    /// <code>
    /// string fullPathToPlayer = GDirectories.FindFile("res://", "Player.tscn")
    /// </code>
    /// </summary>
    /// <returns>Returns the full path to the file or null if the file is not found</returns>
    public static string FindFile(string directory, string fileName)
    {
        directory = NormalizePath(ProjectSettings.GlobalizePath(directory));

        using DirAccess dir = DirAccess.Open(directory);

        dir.ListDirBegin();

        string nextFileName;

        while ((nextFileName = dir.GetNext()) != string.Empty)
        {
            string fullFilePath = Path.Combine(directory, nextFileName);

            if (dir.CurrentIsDir())
            {
                if (!nextFileName.StartsWith('.'))
                {
                    string result = FindFile(fullFilePath, fileName);

                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            else
            {
                if (fileName == nextFileName)
                {
                    return fullFilePath;
                }
            }
        }

        dir.ListDirEnd();

        return null;
    }

    /// <summary>
    /// Removes a specified <paramref name="segmentToRemove"/> from a file <paramref name="path"/>.
    /// <code>
    /// string path = @"A/B/C/D/E";
    /// string segmentToRemove = "C";
    /// string newPath = GDirectories.RemovePathSegment(path, segmentToRemove);
    /// // newPath will be "A/B/D/E"
    /// </code>
    /// </summary>
    /// <param name="path">The original file path.</param>
    /// <param name="segmentToRemove">The segment to remove from the path.</param>
    /// <returns>A new file path with the specified segment removed.</returns>
    public static string RemovePathSegment(string path, string segmentToRemove)
    {
        // Normalize the path separators to match the current environment
        path = NormalizePath(path);
        segmentToRemove = NormalizePath(segmentToRemove);

        // Check if the segment to remove is part of the path
        if (path.Contains(segmentToRemove))
        {
            // Remove the segment from the path
            path = path.Replace(segmentToRemove + Path.DirectorySeparatorChar, string.Empty);
        }

        return path;
    }

    /// <summary>
    /// Normalizes the specified path by replacing all forward slashes ('/') and backslashes ('\') with the system's directory separator character.
    /// </summary>
    /// <param name="path">The path to normalize.</param>
    /// <returns>A normalized path where all directory separators are replaced with the system's directory separator character.</returns>
    /// <remarks>
    /// This method is useful when dealing with paths that may come from different sources (e.g., URLs, different operating systems) and need to be standardized for the current environment.
    /// </remarks>
    public static string NormalizePath(string path)
    {
        return path
            .Replace('/', Path.DirectorySeparatorChar)
            .Replace('\\', Path.DirectorySeparatorChar);
    }

    /// <summary>
    /// Recursively deletes all empty folders in this folder
    /// </summary>
    public static void DeleteEmptyDirectories(string path)
    {
        path = NormalizePath(ProjectSettings.GlobalizePath(path));

        if (Directory.Exists(path))
        {
            foreach (string directory in Directory.GetDirectories(path))
            {
                DeleteEmptyDirectories(directory);
                DeleteEmptyDirectory(directory);
            }
        }
    }

    /// <summary>
    /// Checks if the folder is empty and deletes it if it is
    /// </summary>
    public static void DeleteEmptyDirectory(string path)
    {
        path = NormalizePath(ProjectSettings.GlobalizePath(path));

        if (IsEmptyDirectory(path))
        {
            Directory.Delete(path, recursive: false);
        }
    }

    /// <summary>
    /// Checks if the directory is empty
    /// </summary>
    /// <returns>Returns true if the directory is empty</returns>
    public static bool IsEmptyDirectory(string path)
    {
        path = NormalizePath(ProjectSettings.GlobalizePath(path));

        return Directory.GetDirectories(path).Length == 0 && Directory.GetFiles(path).Length == 0;
    }
}

