using Godot;

namespace Template.Inventory;

[SceneTree]
public partial class ItemContainer : PanelContainer
{
    public void SetItem(ItemStack itemStack)
    {
        if (itemStack != null)
        {
            Item item = ItemInformation.Get(itemStack.Material);

            SetSpriteFrames(item.Resource);
            SetColor(item.Color);
            SetCount(itemStack.Count);
        }
        else
        {
            ClearItem();
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

    private void ClearItem()
    {
        SetSpriteFrames(null);
        SetColor(default);
        SetCount(0);
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
