using Godot;

namespace Template;

public partial class UIOptions : PanelContainer
{
    public override void _Ready()
    {
        if (ServiceProvider.Services.Get<SceneManager>().CurrentScene.Name != "Options")
        {
            GetNode<TextureRect>("BackgroundArt").Hide();
        }
    }
}

