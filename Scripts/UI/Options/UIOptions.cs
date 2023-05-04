namespace Template;

public partial class UIOptions : PanelContainer
{
    public override void _Ready()
    {
        if (SceneManager.CurrentScene.Name != "Options")
            GetNode<TextureRect>("BackgroundArt").Hide();
    }
}
