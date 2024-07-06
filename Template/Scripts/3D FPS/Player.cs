namespace Template.FPS3D;

public partial class Player : CharacterBody3D
{
    [Export] OptionsManager options;

    float mouseSensitivity;
    float gravityForce = 10;
    float jumpForce = 150;
    float moveSpeed = 10;
    float moveDampening = 20; // the higher the value, the less the player will slide
                                   
    Camera3D camera;
    Vector2 mouseInput;
    Vector3 cameraTarget;
    Vector3 cameraOffset;
    Vector3 gravityVec;
    Vector3 camOffset;

    public override async void _Ready()
    {
        camera = GetNode<Camera3D>("%Camera3D");
        camOffset = camera.Position - Position;

        mouseSensitivity = options.Options.MouseSensitivity * 0.0001f;

        await GU.WaitOneFrame(this);

        UIOptionsGameplay gameplay = GetNode<UIPopupMenu>("%PopupMenu")
            .Options.GetNode<UIOptionsGameplay>("%Gameplay");

        gameplay.OnMouseSensitivityChanged += value =>
        {
            mouseSensitivity = value * 0.0001f;
        };
    }

    public override void _PhysicsProcess(double d)
    {
        float delta = (float)d;

        camera.Position = Position + camOffset;
        camera.Rotation = cameraTarget + cameraOffset;

        float h_rot = camera.Basis.GetEuler().Y;

        float f_input = -Input.GetAxis("move_down", "move_up");
        float h_input = Input.GetAxis("move_left", "move_right");

        Vector3 dir = new Vector3(h_input, 0, f_input)
            .Rotated(Vector3.Up, h_rot) // Always face correct direction
            .Normalized(); // Prevent fast strafing movement

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
        if (@event is not InputEventMouseMotion motion || Input.MouseMode != Input.MouseModeEnum.Captured)
            return;

        mouseInput = new Vector2(motion.Relative.X, motion.Relative.Y);

        cameraTarget += new Vector3(-motion.Relative.Y * mouseSensitivity, -motion.Relative.X * mouseSensitivity, 0);

        // prevent camera from looking too far up or down
        Vector3 rotDeg = cameraTarget;
        rotDeg.X = Mathf.Clamp(rotDeg.X, -89f.ToRadians(), 89f.ToRadians());
        cameraTarget = rotDeg;
    }
}
