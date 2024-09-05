using CSharpUtils;

namespace Template.Platformer2D;

public partial class Player
{
    State Slide()
    {
        State state = new(nameof(Slide));

        state.Enter = () =>
        {

        };

        state.Update = delta =>
        {

        };

        state.Transitions = () =>
        {

        };

        return state;
    }
}

