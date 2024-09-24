using Godot;
using System;

namespace Template.InventoryV2;

[SceneTree]
public partial class ItemContainer : PanelContainer
{
    public override void _Ready()
    {
        Count.Hide();
    }

    public void SetSpriteFrames(string resourcePath)
    {
        Sprite.SpriteFrames = SpriteFramesLoader.Load(resourcePath);
        Sprite.Play();
    }

    public void SetCount(int count)
    {
        if (count > 0)
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
