using Godot;
using GodotUtils;

namespace Template;

[Tool]
public partial class SetupToolScript : Node
{
    [Export] public bool RemoveEmptyFolders
    {
        get => false;
        set
        {
            if (Engine.IsEditorHint()) // Do not trigger on game build
            {
                GDirectories.DeleteEmptyDirectories(ProjectSettings.GlobalizePath("res://"));
                GD.Print("Removed all empty folders from the project");
            }
        }
    }
}

