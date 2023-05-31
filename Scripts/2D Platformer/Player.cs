namespace Template.Platformer2D;

public partial class Player : Entity
{
    public PlayerJumpVars JumpVars { get; } = new();

    float maxSpeed = 500;
    float acceleration = 40;
    float friction = 20;
    float gravity = 20;

    public override void Init()
    {
        CurState = new PlayerStateIdle { Player = this };
    }

    public override void Update()
    {
        var vel = Velocity;
        var horzDir = Input.GetAxis("move_left", "move_right");

        // Horizontal movement
        vel.X += horzDir * acceleration;
        vel.X = Utils.ClampAndDampen(vel.X, friction, maxSpeed);

        // Gravity
        vel.Y += gravity;

        Velocity = vel;
    }
}

public class PlayerJumpVars
{
    public float Force { get; } = 100;
    public float Loss { get; } = 7.5f;
    public float LossBuildUp { get; set; }
    public bool HoldingKey { get; set; } // the jump key
}
