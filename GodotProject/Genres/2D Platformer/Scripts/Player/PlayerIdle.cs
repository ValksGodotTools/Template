using CSharpUtils;
using Godot;

namespace Template.Platformer2D.Retro;

public partial class Player
{
    State Idle()
    {
        State state = new(nameof(Idle));

        state.Enter = () =>
        {
            Sprite.Play("idle");
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

