namespace GodotUtils.World2D.Platformer;

using Godot;

public partial class CameraController : Camera2D
{
    float zoomIncrement = 0.08f;
    float minZoom = 1.5f;
    float maxZoom = 3.0f;
    float smoothFactor = 0.25f;
    float horizontalPanSpeed = 8;

    float targetZoom;

    public override void _Ready()
    {
        // Set the initial target zoom value on game start
        targetZoom = base.Zoom.X;
    }

    public override void _PhysicsProcess(double delta)
    {
        float cameraWidth = GetViewportRect().Size.X / base.Zoom.X;
        float camLeftPos = Position.X - (cameraWidth / 2);
        float camRightPos = Position.X + (cameraWidth / 2);

        Panning(camLeftPos, camRightPos);
        Zooming();
        Boundaries(camLeftPos, camRightPos);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent)
            InputEventMouseButton(mouseEvent);

        @event.Dispose(); // Object count was increasing a lot when this function was executed
    }

    void Panning(float camLeftPos, float camRightPos)
    {
        if (Input.IsActionPressed("move_left"))
        {
            // Prevent the camera from going too far left
            if (camLeftPos > LimitLeft)
                Position -= new Vector2(horizontalPanSpeed, 0);
        }

        if (Input.IsActionPressed("move_right"))
        {
            // Prevent the camera from going too far right
            if (camRightPos < LimitRight)
                Position += new Vector2(horizontalPanSpeed, 0);
        }
    }

    void Zooming()
    {
        // Lerp to the target zoom for a smooth effect
        Zoom = Zoom.Lerp(new Vector2(targetZoom, targetZoom), smoothFactor);
    }

    void Boundaries(float camLeftPos, float camRightPos)
    {
        if (camLeftPos < LimitLeft)
        {
            // Travelled this many pixels too far
            float gapDifference = Mathf.Abs(camLeftPos - LimitLeft);

            // Correct position
            Position += new Vector2(gapDifference, 0);
        }

        if (camRightPos > LimitRight)
        {
            // Travelled this many pixels too far
            float gapDifference = Mathf.Abs(camRightPos - LimitRight);

            // Correct position
            Position -= new Vector2(gapDifference, 0);
        }
    }

    void InputEventMouseButton(InputEventMouseButton @event)
    {
        InputZoom(@event);
    }

    void InputZoom(InputEventMouseButton @event)
    {
        // Not sure why or if this is required
        if (!@event.IsPressed())
            return;

        // Zoom in
        if (@event.ButtonIndex == MouseButton.WheelUp)
            targetZoom += zoomIncrement;

        // Zoom out
        if (@event.ButtonIndex == MouseButton.WheelDown)
            targetZoom -= zoomIncrement;

        // Clamp the zoom
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
    }
}
