using Godot.Collections;
using Godot;

namespace Template;

public partial class ResourceHotkeys : Resource
{
    [Export] public Dictionary<StringName, Array<InputEvent>> Actions { get; set; } = new();
}

