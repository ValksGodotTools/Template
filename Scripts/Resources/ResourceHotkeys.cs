namespace Template;

using Godot.Collections;

public partial class ResourceHotkeys : Resource
{
    [Export] public Dictionary<StringName, Array<InputEvent>> Actions { get; set; } = new();
}
