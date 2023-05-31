namespace Template.Platformer2D;

public partial class Player : CharacterBody2D
{
    float maxSpeed = 500;
    float acceleration = 40;
    float friction = 20;
    float gravity = 20;
    float jumpForce = 100;
    float jumpLoss = 7.5f;

    float jumpLossBuildUp;
    bool holdingJumpKey;

    public override void _PhysicsProcess(double delta)
    {
        var horzDir = Input.GetAxis("move_left", "move_right");
        var vel = Velocity;

        // Horizontal movement
        vel.X += horzDir * acceleration;
        vel.X = Utils.ClampAndDampen(vel.X, friction, maxSpeed);

        if (Input.IsActionJustPressed("jump") && IsOnFloor())
        {
            holdingJumpKey = true;
            jumpLossBuildUp = 0;
            vel.Y -= jumpForce;
        }

        if (Input.IsActionPressed("jump") && holdingJumpKey)
        {
            jumpLossBuildUp += jumpLoss;
            vel.Y -= Mathf.Max(0, jumpForce - jumpLossBuildUp);
        }

        if (Input.IsActionJustReleased("jump"))
        {
            holdingJumpKey = false;
        }

        // Gravity
        vel.Y += gravity;

        Velocity = vel;
        MoveAndSlide();
    }
}
