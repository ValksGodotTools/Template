namespace Template.TopDown2D;

public partial class Player : CharacterBody2D
{
    float speed = 50;
    float friction = 0.1f;

    public override void _PhysicsProcess(double delta)
    {
        // Velocity is mutiplied by delta for us already
        Velocity += GetMovementInput() * speed;
        Velocity = Velocity.Lerp(Vector2.Zero, friction);

        MoveAndSlide();
    }

    public Vector2 GetMovementInput(string prefix = "")
    {
        prefix += !string.IsNullOrWhiteSpace(prefix) ? "_" : "";

        return Input.GetVector(
            $"{prefix}move_left", $"{prefix}move_right",
            $"{prefix}move_up", $"{prefix}move_down");
    }

    public Vector2 GetMovementInputRaw(string prefix = "") => 
        GetMovementInput(prefix).Round();
}
