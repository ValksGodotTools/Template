using Godot;

namespace Template.TopDown2D;

public partial class Frog : RigidBody2D
{
    [OnInstantiate]
    private void Init(Vector2 position)
    {
        Position = position;
    }
}
