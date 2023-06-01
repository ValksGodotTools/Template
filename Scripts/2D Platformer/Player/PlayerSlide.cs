namespace Template.Platformer2D;

public partial class Player
{
    State slide;

    void StateSlide()
    {
        slide = new(this, "Slide");

        slide.Enter = () =>
        {

        };


        slide.Update = () =>
        {

        };


        slide.Transitions = () =>
        {

        };
    }
}
