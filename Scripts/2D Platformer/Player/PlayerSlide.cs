namespace Template.Platformer2D;

public partial class Player
{
    State Slide()
    {
        var state = new State(this, "Slide");

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
