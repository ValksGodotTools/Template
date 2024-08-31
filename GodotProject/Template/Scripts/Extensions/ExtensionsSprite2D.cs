namespace GodotUtils;

using Godot;

public static class ExtensionsSprite2D
{
    /// <summary>
    /// The sprite size multiplied by the sprites scale
    /// </summary>
    public static Vector2 GetScaledSize(this Sprite2D sprite) =>
        sprite.GetSize() * sprite.Scale;

    public static Vector2 GetSize(this Sprite2D sprite) => sprite.Texture.GetSize();

    /// <summary>
    /// <para>
    /// Gets the actual pixel size of the sprite. All rows and columns 
    /// consisting of transparent pixels are subtracted from the size.
    /// </para>
    /// 
    /// <para>
    /// This is useful to know if dynamically creating collision
    /// shapes at runtime.
    /// </para>
    /// </summary>
    public static Vector2 GetPixelSize(this Sprite2D sprite) =>
        new Vector2(GetPixelWidth(sprite), GetPixelHeight(sprite));

    /// <summary>
    /// <para>
    /// Gets the actual pixel width of the sprite. All columns consisting of 
    /// transparent pixels are subtracted from the width.
    /// </para>
    /// 
    /// <para>
    /// This is useful to know if dynamically creating collision
    /// shapes at runtime.
    /// </para>
    /// </summary>
    public static int GetPixelWidth(this Sprite2D sprite)
    {
        Image img = sprite.Texture.GetImage();
        Vector2I size = img.GetSize();

        int transColumnsLeft = GImageUtils.GetTransparentColumnsLeft(img, size);
        int transColumnsRight = GImageUtils.GetTransparentColumnsRight(img, size);

        int pixelWidth = size.X - transColumnsLeft - transColumnsRight;

        return (int)(pixelWidth * sprite.Scale.X);
    }

    /// <summary>
    /// <para>
    /// Gets the actual pixel height of the sprite. All rows consisting of 
    /// transparent pixels are subtracted from the height.
    /// </para>
    /// 
    /// <para>
    /// This is useful to know if dynamically creating collision
    /// shapes at runtime.
    /// </para>
    /// </summary>
    public static int GetPixelHeight(this Sprite2D sprite)
    {
        Image img = sprite.Texture.GetImage();
        Vector2I size = img.GetSize();

        int transRowsTop = GImageUtils.GetTransparentRowsTop(img, size);
        int transRowsBottom = GImageUtils.GetTransparentRowsBottom(img, size);

        int pixelHeight = size.Y - transRowsTop - transRowsBottom;

        return (int)(pixelHeight * sprite.Scale.Y);
    }

    public static int GetPixelBottomY(this Sprite2D sprite)
    {
        Image img = sprite.Texture.GetImage();
        Vector2I size = img.GetSize();

        // Might not work with all sprites but works with ninja.
        // The -2 offset that is
        int diff = 0;

        for (int y = (int)size.Y - 1; y >= 0; y--)
        {
            if (img.GetPixel((int)size.X / 2, y).A != 0)
                break;

            diff++;
        }

        return diff;
    }
}
