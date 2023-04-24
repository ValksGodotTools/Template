namespace Template.Platformer2D;

public partial class Player : CharacterBody2D
{
    private float Speed     { get; } = 10;
    private float Gravity   { get; } = 10;
    private float JumpForce { get; } = 500;

    public override void _Ready()
    {
        MotionMode = MotionModeEnum.Grounded;
    }

    public override void _PhysicsProcess(double delta)
    {
        var vel = Velocity;

        if (Input.IsActionPressed("move_left"))
        {
            vel.X -= Speed;
        }

        if (Input.IsActionPressed("move_right"))
        {
            vel.X += Speed;
        }

        if (Input.IsActionJustPressed("jump"))
        {
            vel.Y -= JumpForce;
        }

        vel.Y += Gravity;

        Velocity = vel;
        MoveAndSlide();
    }
}
