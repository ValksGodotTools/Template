using System;

namespace Template;

[AttributeUsage(AttributeTargets.Class)]
public class DraggableAttribute(DragType dragType = DragType.Hold, DragConstraints dragConstraints = DragConstraints.None) : Attribute
{
	public DragType DragType { get; private set; } = dragType;
    public DragConstraints DragConstraints { get; private set; } = dragConstraints;
}

public enum DragType
{
    Hold,
    Click
}

public enum DragConstraints
{
    None,
    Horizontal,
    Vertical
}
