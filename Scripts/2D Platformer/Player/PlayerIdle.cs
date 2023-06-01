namespace Template.Platformer2D;

public partial class Player
{
    State idle;

    void StateIdle()
    {
        idle = new(this, "Idle");

        idle.Enter = () =>
        {
            sprite.Play("idle");
        };

        idle.Transitions = () =>
        {
            if (Input.IsActionJustPressed("jump") && IsOnFloor())
            {
                SwitchState(jump);
            }

            else if (Input.IsActionJustPressed("move_down") && GetFloorAngle() > 0)
            {

            }
        };
    }
}
