using Godot;

namespace Template.FPS3D;

public class Item(Node3D parent) // An item the player can hold
{
    public Skeleton3D SkeletonRig { get; set; } = parent.GetNode<Skeleton3D>("Armature/Skeleton3D");
    public AnimationPlayer AnimationPlayer { get; set; } = parent.GetNode<AnimationPlayer>("AnimationPlayer");

    readonly Node3D parent = parent;

    public void SetVisible(bool v)
    {
        parent.Visible = v;
    }
}

