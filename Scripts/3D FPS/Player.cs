namespace Template.FPS3D;

public partial class Player : CharacterBody3D
{
    float mouseSensitivity = 0.005f;
    float gravityForce = 10;
    float jumpForce = 150;
    float moveSpeed = 10;
    float moveDampening = 20; // the higher the value, the less the player will slide
                                   
    Camera3D camera;
    Vector2 mouseInput;
    Vector3 cameraTarget;
    Vector3 cameraOffset;
    Vector3 gravityVec;

    UIPopupMenu popupMenu;

    public override void _Ready()
    {
        camera = GetNode<Camera3D>("Camera3D");
        popupMenu = GetNode<UIPopupMenu>("%PopupMenu");
        popupMenu.OnOpened += () =>
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
        };
        popupMenu.OnClosed += () =>
        {
            Input.MouseMode = Input.MouseModeEnum.Captured;
        };

        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _PhysicsProcess(double d)
    {
        var delta = (float)d;

        camera.Rotation = cameraTarget + cameraOffset;

        //var h_rot = GlobalTransform.basis.GetEuler().y;
        float h_rot = camera.Basis.GetEuler().Y;

        float f_input = -Input.GetAxis("move_down", "move_up");
        float h_input = Input.GetAxis("move_left", "move_right");

        // Normalized to prevent "fast strafing movement" by holding down 2
        // movement keys at the same time
        // Rotated to horizontal rotation to always move in the correct
        // direction
        var dir = new Vector3(h_input, 0, f_input)
            .Rotated(Vector3.Up, h_rot)
            .Normalized();

        if (IsOnFloor())
        {
            gravityVec = Vector3.Zero;

            if (Input.IsActionJustPressed("jump"))
            {
                gravityVec = Vector3.Up * jumpForce * delta;
            }
        }
        else
        {
            gravityVec += Vector3.Down * gravityForce * delta;
        }

        Velocity = Velocity.Lerp(dir * moveSpeed, moveDampening * delta);
        Velocity += gravityVec;

        MoveAndSlide();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is not InputEventMouseMotion motion ||
            Input.MouseMode != Input.MouseModeEnum.Captured)
            return;

        mouseInput = new Vector2(motion.Relative.X, motion.Relative.Y);

        cameraTarget += new Vector3(-motion.Relative.Y * mouseSensitivity, -motion.Relative.X * mouseSensitivity, 0);

        // prevent camera from looking too far up or down
        Vector3 rotDeg = cameraTarget;
        rotDeg.X = Mathf.Clamp(rotDeg.X, -89f.ToRadians(), 89f.ToRadians());
        cameraTarget = rotDeg;
    }
}
