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
        if (Services.Get<SceneManager>().CurrentScene.Name != "Options")
        {
            _.BackgroundArt.Hide();
        }
    }
}
