namespace Template.Platformer2D;

public partial class Player : CharacterBody2D
{
    private float MaxSpeed      { get; } = 500;
    private float Acceleration  { get; } = 40;
    private float Friction      { get; } = 20;
    private float Gravity       { get; } = 20;
    private float JumpForce     { get; } = 100;
    private float JumpLoss      { get; } = 7.5f;

    private float JumpLossBuildUp { get; set; }
    private bool  HoldingJumpKey  { get; set; }

    private Dictionary<string, RayCast2D[]> RayCasts { get; set; } = new();

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
            HoldingJumpKey = true;
            JumpLossBuildUp = 0;
            vel.Y -= JumpForce;
        }

        if (Input.IsActionPressed("jump") && HoldingJumpKey)
        {
            JumpLossBuildUp += JumpLoss;
            vel.Y -= Mathf.Max(0, JumpForce - JumpLossBuildUp);
        }

        if (Input.IsActionJustReleased("jump"))
        {
            HoldingJumpKey = false;
        }

        // Gravity
        vel.Y += Gravity;

        Velocity = vel;
        MoveAndSlide();
    }

    private void PrepareRaycasts(string type)
    {
        var raycastsFloor = GetNode($"Raycasts/{type}").GetChildren<RayCast2D>();

        RayCasts[type] = new RayCast2D[raycastsFloor.Length];

        for (int i = 0; i < raycastsFloor.Length; i++)
        {
            RayCasts[type][i] = raycastsFloor[i];
            RayCasts[type][i].AddException(this);
        }
    }

    private bool AreRaycastsTouching(string type)
    {
        foreach (var raycast in RayCasts[type])
            if (raycast.IsColliding())
                return true;

        return false;
    }
}
