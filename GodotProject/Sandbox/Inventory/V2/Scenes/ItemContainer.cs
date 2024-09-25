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
        if (item != null)
        {
            SetSpriteFrames(item.ResourcePath);
            SetColor(item.Color);
            SetCount(item.Count);
        }
        else
        {
            SetSpriteFrames(null);
            SetColor(default);
            SetCount(0);
        }
    }

    private void SetSpriteFrames(string resourcePath)
    {
        if (!string.IsNullOrWhiteSpace(resourcePath))
        {
            Sprite.SpriteFrames = SpriteFramesLoader.Load(resourcePath);
            Sprite.Play();
        }
        else
        {
            Sprite.SpriteFrames = null;
        }
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
