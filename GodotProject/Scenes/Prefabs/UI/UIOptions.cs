using Godot;

namespace Template;

public partial class UIOptions : PanelContainer
{
    [OnInstantiate]
    private void Init()
    {

    }

    public override void _Ready()
    {
        if (ServiceProvider.Services.Get<SceneManager>().CurrentScene.Name != "Options")
        {
            GetNode<TextureRect>("BackgroundArt").Hide();
        }
    }
}

