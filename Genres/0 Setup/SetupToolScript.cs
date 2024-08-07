namespace Template;

[Tool]
public partial class SetupToolScript : Node
{
    [Export] public bool RemoveEmptyFolders
    {
        get => _removeEmptyFolders;
        set
        {
            _removeEmptyFolders = value;

            if (_removeEmptyFolders)
            {
                GD.Print("Removing empty folders");
                GU.DeleteEmptyFolders(ProjectSettings.GlobalizePath("res://"));
            }
        }
    }

    bool _removeEmptyFolders;
}
