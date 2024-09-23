namespace Template.Inventory;

public static class Items
{
    public static readonly Item Coin = CreateCoin();
    public static readonly Item SnowyCoin = CreateSnowyCoin();

    private static Item CreateCoin()
    {
        Item item = new("Coin");
        item.Texture = ItemTexture.CoinAnimated;
        return item;
    }

    private static Item CreateSnowyCoin()
    {
        Item item = new("Snowy Coin");
        item.Texture = ItemTexture.CoinStatic;
        return item;
    }
}
