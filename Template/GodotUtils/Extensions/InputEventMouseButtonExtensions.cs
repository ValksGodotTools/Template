using Godot;

namespace GodotUtils;

public static class InputEventMouseButtonExtensions
{
    // Zoom
    public static bool IsWheelUp(this InputEventMouseButton @event) => IsZoomIn(@event);
    public static bool IsWheelDown(this InputEventMouseButton @event) => IsZoomOut(@event);

    /// <summary>Returns true if a mouse WheelUp event was detected</summary>
    public static bool IsZoomIn(this InputEventMouseButton @event) => @event.IsPressed(MouseButton.WheelUp);

    /// <summary>Returns true if a mouse WheelDown event was detected</summary>
    public static bool IsZoomOut(this InputEventMouseButton @event) => @event.IsPressed(MouseButton.WheelDown);

    // Pressed
    public static bool IsLeftClickPressed(this InputEventMouseButton @event) => @event.IsPressed(MouseButton.Left);
    public static bool IsLeftClickJustPressed(this InputEventMouseButton @event) => @event.IsJustPressed(MouseButton.Left);
    public static bool IsRightClickPressed(this InputEventMouseButton @event) => @event.IsPressed(MouseButton.Right);
    public static bool IsRightClickJustPressed(this InputEventMouseButton @event) => @event.IsJustPressed(MouseButton.Right);

    // Released
    public static bool IsLeftClickReleased(this InputEventMouseButton @event) => @event.IsReleased(MouseButton.Left);
    public static bool IsLeftClickJustReleased(this InputEventMouseButton @event) => @event.IsJustReleased(MouseButton.Left);
    public static bool IsRightClickReleased(this InputEventMouseButton @event) => @event.IsReleased(MouseButton.Right);
    public static bool IsRightClickJustReleased(this InputEventMouseButton @event) => @event.IsJustReleased(MouseButton.Right);

    // Helper Functions
    private static bool IsPressed(this InputEventMouseButton @event, MouseButton button) => @event.ButtonIndex == button && @event.Pressed;
    private static bool IsJustPressed(this InputEventMouseButton @event, MouseButton button) => @event.IsPressed(button) && !@event.IsEcho();
    private static bool IsReleased(this InputEventMouseButton @event, MouseButton button) => @event.ButtonIndex == button && !@event.Pressed;
    private static bool IsJustReleased(this InputEventMouseButton @event, MouseButton button) => @event.IsReleased(button) && !@event.IsEcho();
}

