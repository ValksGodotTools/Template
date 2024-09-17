using CSharpUtils;
using Godot;
using Template.TopDown2D;

namespace Template;

public abstract partial class NodeState : Node
{
    public virtual Player Player { get; set; }

    public abstract State GetState();
}
