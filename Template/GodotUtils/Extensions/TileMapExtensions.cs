using Godot;

namespace GodotUtils;

public static class TileMapExtensions
{
    /// <summary>
    /// Retrieves the custom data of type <typeparamref name="T"/> set at <paramref name="tileCoordinates"/>. 
    /// 
    /// <para>
    /// Suppose we setup a new Custom Data Layer on our tilemap named "name" and we use this 
    /// data layer to paint the name data (e.g. "floor", "wall", "lava") onto each tile.
    /// </para>
    /// 
    /// <code>
    /// string tileName = tileMap.GetCustomData&lt;string&gt;(new Vector2I(x, y), "name");
    /// 
    /// // If no data is at this tile then "" will be printed
    /// GD.Print(tileName);
    /// </code>
    /// </summary>
    public static T GetCustomData<[MustBeVariant]T>(this TileMapLayer tileMap, Vector2I tileCoordinates, string customDataLayerName)
    {
        TileData tileData = tileMap.GetCellTileData(tileCoordinates);

        if (tileData != null)
        {
            T data = tileData.GetCustomData(customDataLayerName).As<T>();
            return data;
        }

        return default;
    }
}

