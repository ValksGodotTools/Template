using Godot;

namespace Template.Inventory;

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

    public void HideItem()
    {
        Sprite.Hide();
        Count.Hide();
    }

    public void ShowItem()
    {
        Sprite.Show();
        Count.Show();
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

    public void SetCurrentSpriteFrame(int frame)
    {
        Sprite.Frame = frame;
    }

    public int GetCurrentSpriteFrame()
    {
        return Sprite.Frame;
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
