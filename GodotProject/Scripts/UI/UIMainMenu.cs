using Godot;

namespace Template;

public partial class UIMainMenu : Node
{
    [Export] GameState gameState;

    public override void _Ready()
    {
        //Global.Services.Get<AudioManager>().PlayMusic(Music.Menu);
    }
}

