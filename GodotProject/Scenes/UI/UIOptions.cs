using Godot;

namespace Template;

[SceneTree]
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
            _.BackgroundArt.Hide();
        }
    }
}
