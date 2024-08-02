namespace Template.TopDown2D;

using GodotUtils.World2D.TopDown;

public partial class Player : CharacterBody2D
{
    float speed = 50;
    float friction = 0.1f;

    public override void _PhysicsProcess(double delta)
    {
        MoveAndSlide();

        // Velocity is mutiplied by delta for us already
        Velocity += Utils.GetMovementInput() * speed;
        Velocity = Velocity.Lerp(Vector2.Zero, friction);
    }
}
