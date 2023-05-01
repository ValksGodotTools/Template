namespace Template.FPS3D;

public partial class Player : CharacterBody3D
{
    private float MouseSensitivity { get; set; } = 0.005f;
    private float GravityForce     { get; set; } = 10;
    private float JumpForce        { get; set; } = 150;
    private float MoveSpeed        { get; set; } = 10;
    private float MoveDampening    { get; set; } = 20; // the higher the value, the less the player will slide
                                   
    private Camera3D camera;
    private Vector2 mouseInput;
    private Vector3 cameraTarget;
    private Vector3 cameraOffset;
    private Vector3 gravityVec;
    private bool mouseCaptured;

    public override void _Ready()
    {
        camera = GetNode<Camera3D>("Camera3D");

        Input.MouseMode = Input.MouseModeEnum.Captured;
        mouseCaptured = true;
    }

    public override void _PhysicsProcess(double d)
    {
        var delta = (float)d;

        camera.Rotation = cameraTarget + cameraOffset;

        //var h_rot = GlobalTransform.basis.GetEuler().y;
        var h_rot = camera.Basis.GetEuler().Y;

        var f_input = -Input.GetAxis("move_down", "move_up");
        var h_input = Input.GetAxis("move_left", "move_right");

        // normalized to prevent "fast strafing movement" by holding down 2 movement keys at the same time
        // rotated to horizontal rotation to always move in the correct direction
        var dir = new Vector3(h_input, 0, f_input).Rotated(Vector3.Up, h_rot).Normalized();

        if (IsOnFloor())
        {
            gravityVec = Vector3.Zero;

            if (Input.IsActionJustPressed("jump"))
            {
                gravityVec = Vector3.Up * JumpForce * delta;
            }
        }
        else
        {
            gravityVec += Vector3.Down * GravityForce * delta;
        }

        Velocity = Velocity.Lerp(dir * MoveSpeed, MoveDampening * delta);
        Velocity += gravityVec;

        MoveAndSlide();
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            mouseCaptured = !mouseCaptured;

            Input.MouseMode = mouseCaptured ?
                Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible;
        }

        if (@event is not InputEventMouseMotion motion)
            return;

        if (Input.MouseMode != Input.MouseModeEnum.Captured)
            return;

        mouseInput = new Vector2(motion.Relative.X, motion.Relative.Y);

        cameraTarget += new Vector3(-motion.Relative.Y * MouseSensitivity, -motion.Relative.X * MouseSensitivity, 0);

        // prevent camera from looking too far up or down
        var rotDeg = cameraTarget;
        rotDeg.X = Mathf.Clamp(rotDeg.X, -89f.ToRadians(), 89f.ToRadians());
        cameraTarget = rotDeg;
    }
}
