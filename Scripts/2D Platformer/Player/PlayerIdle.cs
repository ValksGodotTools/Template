namespace Template.Platformer2D;

public partial class Player
{
    State Idle()
    {
        var state = new State(this, nameof(Idle));

        state.Enter = () =>
        {
            sprite.Play("idle");
        };

        state.Transitions = () =>
        {
            if (Input.IsActionJustPressed("jump") && IsOnFloor())
            {
                SwitchState(Jump());
            }

            else if (Input.IsActionJustPressed("move_down") && GetFloorAngle() > 0)
            {

            }
        };

        return state;
    }
}
