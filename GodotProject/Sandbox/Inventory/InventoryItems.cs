using Godot;

namespace Template.Inventory;

public static class InventoryItems
{
    public static UIInventoryItemSprite YellowCoin()
    {
        SpriteFrames spriteFrames = GD.Load<SpriteFrames>("res://Sandbox/Inventory/CoinSpriteFrames.tres");

        return new UIInventoryItemSprite(spriteFrames)
            .SetColor(Colors.Yellow)
            .SetScale(2);
    }
}
