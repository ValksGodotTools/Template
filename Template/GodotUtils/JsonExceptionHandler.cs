using Godot;
using System.IO;
using System.Text;
using System;
using System.Text.Json;

namespace GodotUtils;

public class JsonExceptionHandler
{
    public static void Handle(JsonException ex, string jsonText, string path)
    {
        // Extract relevant information from the exception
        long? lineNumber = ex.LineNumber;

        if (lineNumber.HasValue)
        {
            // Split the JSON into lines
            string[] lines = jsonText.Split('\n');

            // Get the problematic line
            string problematicLine = lines[lineNumber.Value - 1];

            // Determine the range of lines to display
            int startLine = Math.Max(0, (int)lineNumber.Value - 7);
            int endLine = Math.Min(lines.Length, (int)lineNumber.Value + 7);

            // Create the error message
            StringBuilder errorMessage = new();

            errorMessage.AppendLine($"ERROR: Failed to parse {Path.GetFileName(path)}");
            errorMessage.AppendLine();
            errorMessage.AppendLine($"{ex.Message}");
            errorMessage.AppendLine();

            // Add the lines before the problematic line
            for (int i = startLine; i < lineNumber.Value - 1; i++)
            {
                errorMessage.AppendLine(lines[i]);
            }

            // Add the problematic line with the caret indicating the error position
            errorMessage.AppendLine($"{problematicLine} <--- Syntax error could be on this line or the next line");

            // Add the lines after the problematic line
            for (int i = (int)lineNumber.Value; i < endLine; i++)
            {
                errorMessage.AppendLine(lines[i]);
            }

            GD.Print(errorMessage);
        }
        else
        {
            GD.Print($"ERROR: Failed to parse {Path.GetFileName(path)}");
            GD.Print(ex.Message);
        }
    }
}
