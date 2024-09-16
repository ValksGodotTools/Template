using Godot;

namespace Template;

public partial class UIMainMenu : Node
{
    [Export] private GameState gameState;

    public override void _Ready()
    {
        //Global.Services.Get<AudioManager>().PlayMusic(Music.Menu);
    }
}

