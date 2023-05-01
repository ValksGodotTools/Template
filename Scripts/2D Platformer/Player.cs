namespace Template.Platformer2D;

public partial class Player : CharacterBody2D
{
    private float MaxSpeed      { get; } = 500;
    private float Acceleration  { get; } = 40;
    private float Friction      { get; } = 20;
    private float Gravity       { get; } = 20;
    private float JumpForce     { get; } = 100;
    private float JumpLoss      { get; } = 7.5f;

    private float jumpLossBuildUp;
    private bool holdingJumpKey;
    private readonly Dictionary<string, RayCast2D[]> rayCasts = new();

    public override void _Ready()
    {
        MotionMode = MotionModeEnum.Grounded;

        PrepareRaycasts("Floor");
        PrepareRaycasts("Wall Left");
        PrepareRaycasts("Wall Right");
    }

    public override void _PhysicsProcess(double delta)
    {
        var horzDir = Input.GetAxis("move_left", "move_right");
        var vel = Velocity;

        // Horizontal movement
        vel.X += horzDir * Acceleration;
        vel.X = Utils.ClampAndDampen(vel.X, Friction, MaxSpeed);

        // Jump
        if (Input.IsActionJustPressed("jump") && AreRaycastsTouching("Floor"))
        {
            holdingJumpKey = true;
            jumpLossBuildUp = 0;
            vel.Y -= JumpForce;
        }

        if (Input.IsActionPressed("jump") && holdingJumpKey)
        {
            jumpLossBuildUp += JumpLoss;
            vel.Y -= Mathf.Max(0, JumpForce - jumpLossBuildUp);
        }

        if (Input.IsActionJustReleased("jump"))
        {
            holdingJumpKey = false;
        }

        // Gravity
        vel.Y += Gravity;

        Velocity = vel;
        MoveAndSlide();
    }

    private void PrepareRaycasts(string type)
    {
        var raycastsFloor = GetNode($"Raycasts/{type}").GetChildren<RayCast2D>();

        rayCasts[type] = new RayCast2D[raycastsFloor.Length];

        for (int i = 0; i < raycastsFloor.Length; i++)
        {
            rayCasts[type][i] = raycastsFloor[i];
            rayCasts[type][i].AddException(this);
        }
    }

    private bool AreRaycastsTouching(string type)
    {
        foreach (var raycast in rayCasts[type])
            if (raycast.IsColliding())
                return true;

        return false;
    }
}
