namespace Template.Platformer2D;

public partial class Player
{
    State Slide()
    {
        State state = new State(this, nameof(Slide));

        state.Enter = () =>
        {

        };


        state.Update = () =>
        {

        };


        state.Transitions = () =>
        {

        };

        return state;
    }
}
