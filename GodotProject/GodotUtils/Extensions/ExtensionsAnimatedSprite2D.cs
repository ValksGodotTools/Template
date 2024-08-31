namespace GodotUtils;

using Godot;

public static class ExtensionsAnimatedSprite2D
{
    /// <summary>
    /// There may be a small delay when switching between animations. Use this function
    /// to remove that delay.
    /// </summary>
    public static void InstantPlay(this AnimatedSprite2D sprite, string anim)
    {
        sprite.Animation = anim;
        sprite.Play(anim);
    }

    /// <summary>
    /// There may be a small delay when switching between animations. Use this function
    /// to remove that delay.
    /// </summary>
    public static void InstantPlay(this AnimatedSprite2D sprite, string anim, int frame)
    {
        sprite.Animation = anim;

        int frameCount = sprite.SpriteFrames.GetFrameCount(anim);

        if (frameCount - 1 >= frame)
            sprite.Frame = frame;
        else
            ServiceProvider.Services.Get<Logger>().LogWarning($"The frame '{frame}' specified for {sprite.Name} is" +
                $"lower than the frame count '{frameCount}'");

        sprite.Play(anim);
    }

    /// <summary>
    /// <para>
    /// Play a animation starting at a random frame
    /// </para>
    /// 
    /// <para>
    /// This is useful if making for example coin animations play at random frames
    /// </para>
    /// </summary>
    public static void PlayRandom(this AnimatedSprite2D sprite, string anim = "")
    {
        anim = string.IsNullOrWhiteSpace(anim) ? sprite.Animation : anim;
        sprite.InstantPlay(anim);
        sprite.Frame = GD.RandRange(0, sprite.SpriteFrames.GetFrameCount(anim));
    }

    /// <summary>
    /// Gets the scaled width of the specified sprite frame
    /// </summary>
    public static int GetWidth(this AnimatedSprite2D sprite, string anim = "")
    {
        anim = string.IsNullOrWhiteSpace(anim) ? sprite.Animation : anim;
        return (int)(sprite.SpriteFrames.GetFrameTexture(anim, 0).GetWidth() *
            sprite.Scale.X);
    }

    /// <summary>
    /// Gets the scaled height of the specified sprite frame
    /// </summary>
    public static int GetHeight(this AnimatedSprite2D sprite, string anim = "")
    {
        anim = string.IsNullOrWhiteSpace(anim) ? sprite.Animation : anim;
        return (int)(sprite.SpriteFrames.GetFrameTexture(anim, 0).GetHeight() *
            sprite.Scale.Y);
    }

    /// <summary>
    /// Gets the scaled size of the specified sprite frame
    /// </summary>
    public static Vector2 GetScaledSize(this AnimatedSprite2D sprite, string anim = "")
    {
        anim = string.IsNullOrWhiteSpace(anim) ? sprite.Animation : anim;
        return new Vector2(GetWidth(sprite, anim), GetHeight(sprite, anim));
    }

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
    public static Vector2 GetPixelSize(this AnimatedSprite2D sprite, string anim = "")
    {
        anim = string.IsNullOrWhiteSpace(anim) ? sprite.Animation : anim;
        return new Vector2(GetPixelWidth(sprite, anim), GetPixelHeight(sprite, anim));
    }

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
    public static int GetPixelWidth(this AnimatedSprite2D sprite, string anim = "")
    {
        anim = string.IsNullOrWhiteSpace(anim) ? sprite.Animation : anim;

        Texture2D tex = sprite.SpriteFrames.GetFrameTexture(anim, 0);
        Image img = tex.GetImage();
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
    public static int GetPixelHeight(this AnimatedSprite2D sprite, string anim = "")
    {
        anim = string.IsNullOrWhiteSpace(anim) ? sprite.Animation : anim;

        Texture2D tex = sprite.SpriteFrames.GetFrameTexture(anim, 0);
        Image img = tex.GetImage();
        Vector2I size = img.GetSize();

        int transRowsTop = GImageUtils.GetTransparentRowsTop(img, size);
        int transRowsBottom = GImageUtils.GetTransparentRowsBottom(img, size);

        int pixelHeight = size.Y - transRowsTop - transRowsBottom;

        return (int)(pixelHeight * sprite.Scale.Y);
    }

    public static int GetPixelBottomY(this AnimatedSprite2D sprite, string anim = "")
    {
        anim = string.IsNullOrWhiteSpace(anim) ? sprite.Animation : anim;

        Texture2D tex = sprite.SpriteFrames.GetFrameTexture(anim, 0);
        Image img = tex.GetImage();
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
