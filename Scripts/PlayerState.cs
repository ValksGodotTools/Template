using Template.Platformer2D;

namespace Template;

public class PlayerState : State
{
    public Player Player
    {
        get => player;
        set {
            player = value;
            Entity = value;
        }
    }

    Player player;
}
