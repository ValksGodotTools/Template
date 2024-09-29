using Godot;

namespace Template.Unorganized.Platformer;

public partial class CameraController : Camera2D
{
    private float _zoomIncrement = 0.08f;
    private float _minZoom = 1.5f;
    private float _maxZoom = 3.0f;
    private float _smoothFactor = 0.25f;
    private float _horizontalPanSpeed = 8;
    private float _targetZoom;

    public override void _Ready()
    {
        // Set the initial target zoom value on game start
        _targetZoom = base.Zoom.X;
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
        {
            InputEventMouseButton(mouseEvent);
        }

        @event.Dispose(); // Object count was increasing a lot when this function was executed
    }

    private void Panning(float camLeftPos, float camRightPos)
    {
        if (Input.IsActionPressed(InputActions.MoveLeft))
        {
            // Prevent the camera from going too far left
            if (camLeftPos > LimitLeft)
            {
                Position -= new Vector2(_horizontalPanSpeed, 0);
            }
        }

        if (Input.IsActionPressed(InputActions.MoveRight))
        {
            // Prevent the camera from going too far right
            if (camRightPos < LimitRight)
            {
                Position += new Vector2(_horizontalPanSpeed, 0);
            }
        }
    }

    private void Zooming()
    {
        // Lerp to the target zoom for a smooth effect
        Zoom = Zoom.Lerp(new Vector2(_targetZoom, _targetZoom), _smoothFactor);
    }

    private void Boundaries(float camLeftPos, float camRightPos)
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

    private void InputEventMouseButton(InputEventMouseButton @event)
    {
        InputZoom(@event);
    }

    private void InputZoom(InputEventMouseButton @event)
    {
        // Not sure why or if this is required
        if (!@event.IsPressed())
        {
            return;
        }

        // Zoom in
        if (@event.ButtonIndex == MouseButton.WheelUp)
        {
            _targetZoom += _zoomIncrement;
        }

        // Zoom out
        if (@event.ButtonIndex == MouseButton.WheelDown)
        {
            _targetZoom -= _zoomIncrement;
        }

        // Clamp the zoom
        _targetZoom = Mathf.Clamp(_targetZoom, _minZoom, _maxZoom);
    }
}

