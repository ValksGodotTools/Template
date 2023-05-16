namespace Template.TopDown2D;

public partial class Player : CharacterBody2D
{
    public float Speed    { get; set; } = 50;
    public float Friction { get; set; } = 0.1f;

    public override void _PhysicsProcess(double delta)
    {
        // Velocity is mutiplied by delta for us already
        Velocity += GetMovementInput() * Speed;
        Velocity = Velocity.Lerp(Vector2.Zero, Friction);

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
