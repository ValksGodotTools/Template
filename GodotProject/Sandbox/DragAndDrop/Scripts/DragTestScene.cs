using Godot;
using System;
using System.Reflection;

namespace Template.DragAndDrop;

public partial class DragTestScene : Node
{
    public override void _Ready()
    {
        Type[] types = Assembly.GetExecutingAssembly().GetTypes();

        foreach (Type type in types)
        {
            DraggableAttribute draggableAttribute = (DraggableAttribute)type.GetCustomAttribute(typeof(DraggableAttribute));

            if (draggableAttribute != null)
            {
                GD.Print(type);
            }
        }
    }
}
