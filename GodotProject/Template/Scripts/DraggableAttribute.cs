using System;

namespace Template;

[AttributeUsage(AttributeTargets.Class)]
public class DraggableAttribute(DragType dragType = DragType.Hold) : Attribute
{
	public DragType DragType { get; private set; } = dragType;
}

public enum DragType
{
    Click,
    Hold
}
