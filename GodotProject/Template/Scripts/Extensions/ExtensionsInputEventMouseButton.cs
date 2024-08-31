namespace GodotUtils;

using Godot;

public static class ExtensionsInputEventMouseButton
{
    public static bool IsWheelUp(this InputEventMouseButton @event) =>
        IsZoomIn(@event);

    public static bool IsWheelDown(this InputEventMouseButton @event) =>
        IsZoomOut(@event);

    /// <summary>
    /// Returns true if a mouse WheelUp event was detected
    /// </summary>
    public static bool IsZoomIn(this InputEventMouseButton @event) =>
        @event.IsPressed(MouseButton.WheelUp);

    /// <summary>
    /// Returns true if a mouse WheelDown event was detected
    /// </summary>
    public static bool IsZoomOut(this InputEventMouseButton @event) =>
        @event.IsPressed(MouseButton.WheelDown);

    public static bool IsLeftClickPressed(this InputEventMouseButton @event) =>
        @event.IsPressed(MouseButton.Left);

    public static bool IsLeftClickReleased(this InputEventMouseButton @event) =>
        @event.IsReleased(MouseButton.Left);

    public static bool IsRightClickPressed(this InputEventMouseButton @event) =>
        @event.IsPressed(MouseButton.Right);

    public static bool IsRightClickReleased(this InputEventMouseButton @event) =>
        @event.IsReleased(MouseButton.Right);

    // Helper Functions
    static bool IsPressed(this InputEventMouseButton @event, MouseButton button) =>
        @event.ButtonIndex == button && @event.Pressed;

    static bool IsReleased(this InputEventMouseButton @event, MouseButton button) =>
        @event.ButtonIndex == button && !@event.Pressed;
}
