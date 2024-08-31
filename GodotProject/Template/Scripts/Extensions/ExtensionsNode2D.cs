namespace GodotUtils;

using Godot;

public static class ExtensionsNode2D
{
    /// <summary>
    /// <para>
    /// Reparent a node to a new parent. The rotation and position will be
    /// preserved.
    /// </para>
    /// 
    /// <para>
    /// Useful for if example you want to detach missiles connected from a
    /// spaceship. The missiles position and rotation will no longer be
    /// influenced by the spaceship.
    /// </para>
    /// </summary>
    public static void Reparent(this Node2D node, Node targetParent)
    {
        Vector2 pos = node.GlobalPosition;
        float rot = node.GlobalRotation;

        node.GetParent().RemoveChild(node);
        targetParent.AddChild(node);

        node.GlobalPosition = pos;
        node.GlobalRotation = rot;
    }
}
