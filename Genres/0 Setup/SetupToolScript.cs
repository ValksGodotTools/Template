namespace Template;

[Tool]
public partial class SetupToolScript : Node
{
    [Export] public bool RemoveEmptyFolders
    {
        get => false;
        set
        {
            GU.DeleteEmptyFolders(ProjectSettings.GlobalizePath("res://"));
            GD.Print("Removed all empty folders from the project");
        }
    }
}
