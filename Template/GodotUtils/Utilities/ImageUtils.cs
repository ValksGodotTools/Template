using Godot;

namespace GodotUtils;

public static class ImageUtils
{
    public static int GetTransparentColumnsLeft(Image img, Vector2 size)
    {
        int columns = 0;

        for (int x = 0; x < size.X; x++)
        {
            for (int y = 0; y < size.Y; y++)
            {
                if (img.GetPixel(x, y).A != 0)
                {
                    return columns;
                }
            }

            columns++;
        }

        return columns;
    }

    public static int GetTransparentColumnsRight(Image img, Vector2 size)
    {
        int columns = 0;

        for (int x = (int)size.X - 1; x >= 0; x--)
        {
            for (int y = 0; y < size.Y; y++)
            {
                if (img.GetPixel(x, y).A != 0)
                {
                    return columns;
                }
            }

            columns++;
        }

        return columns;
    }

    public static int GetTransparentRowsTop(Image img, Vector2 size)
    {
        int rows = 0;

        for (int y = 0; y < size.Y; y++)
        {
            for (int x = 0; x < size.X; x++)
            {
                if (img.GetPixel(x, y).A != 0)
                {
                    return rows;
                }
            }

            rows++;
        }

        return rows;
    }

    public static int GetTransparentRowsBottom(Image img, Vector2 size)
    {
        int rows = 0;

        for (int y = (int)size.Y - 1; y >= 0; y--)
        {
            for (int x = 0; x < size.X; x++)
            {
                if (img.GetPixel(x, y).A != 0)
                {
                    return rows;
                }
            }

            rows++;
        }

        return rows;
    }
}

