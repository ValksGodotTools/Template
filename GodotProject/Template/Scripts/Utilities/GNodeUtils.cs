namespace GodotUtils;

using Godot;

public static class GNodeUtils
{
    public static Area2D CreateAreaRect(Node parent, Vector2 size, string debugColor = "ff001300") =>
        CreateArea(parent, new RectangleShape2D { Size = size }, debugColor);

    public static Area2D CreateAreaCircle(Node parent, float radius, string debugColor = "ff001300") =>
        CreateArea(parent, new CircleShape2D { Radius = radius }, debugColor);

    public static Area2D CreateArea(Node parent, Shape2D shape, string debugColor = "ff001300")
    {
        Area2D area = new Area2D();
        CollisionShape2D areaCollision = new CollisionShape2D
        {
            DebugColor = new Color(debugColor),
            Shape = shape
        };

        area.AddChild(areaCollision);
        parent.AddChild(area);

        return area;
    }
}
