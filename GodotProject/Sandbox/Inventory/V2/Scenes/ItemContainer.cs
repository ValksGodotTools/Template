using Godot;

namespace Template.InventoryV2;

[SceneTree]
public partial class ItemContainer : PanelContainer
{
    public override void _Ready()
    {
        Count.Hide();
    }

    public void SetItem(Item item)
    {
        SetSpriteFrames(item.ResourcePath);
        SetColor(item.Color);
        SetCount(item.Count);
    }

    private void SetSpriteFrames(string resourcePath)
    {
        Sprite.SpriteFrames = SpriteFramesLoader.Load(resourcePath);
        Sprite.Play();
    }

    private void SetColor(Color color)
    {
        Sprite.Modulate = color != default ? color : Colors.White;
    }

    private void SetCount(int count)
    {
        if (count > 1)
        {
            Count.Text = count.ToString();
            Count.Show();
        }
        else
        {
            Count.Hide();
        }
    }

    [OnInstantiate]
    private void Init()
    {
        
    }
}
