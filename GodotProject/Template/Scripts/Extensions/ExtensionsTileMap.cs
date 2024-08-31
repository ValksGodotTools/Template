namespace GodotUtils;

using Godot;

public static class ExtensionsTileMap
{
    public static void EnableLayers(this TileMapLayer tileMap, params int[] layers)
    {
        int result = GMath.GetLayerValues(layers);

        tileMap.TileSet.SetPhysicsLayerCollisionLayer(0, (uint)result);
        tileMap.TileSet.SetPhysicsLayerCollisionMask(0, (uint)result);
    }

    /// <summary>
    /// <para>
    /// Get the tile data from a global position. Use 
    /// tileData.Equals(default(Variant)) to check if no tile data exists here.
    /// </para>
    /// 
    /// <para>
    /// Useful if trying to get the tile the player is currently inside.
    /// </para>
    /// 
    /// <para>
    /// To get the tile the player is standing on see RayCast2D.GetTileData(...)
    /// </para>
    /// </summary>
    public static Variant GetTileData(this TileMapLayer tilemap, Vector2 pos, string layerName)
    {
        Vector2I tilePos = tilemap.LocalToMap(tilemap.ToLocal(pos));

        TileData tileData = tilemap.GetCellTileData(tilePos);

        if (tileData == null)
            return default;

        return tileData.GetCustomData(layerName);
    }

    public static bool InTileMap(this TileMapLayer tilemap, Vector2 pos)
    {
        Vector2I tilePos = tilemap.LocalToMap(tilemap.ToLocal(pos));

        return tilemap.GetCellSourceId(tilePos) != -1;
    }

    public static string GetTileName(this TileMapLayer tilemap, Vector2 pos)
    {
        if (!tilemap.TileExists(pos))
            return "";

        TileData tileData = tilemap.GetCellTileData(tilemap.LocalToMap(pos));

        if (tileData == null)
            return "";

        Variant data = tileData.GetCustomData("Name");

        return data.AsString();
    }

    public static bool TileExists(this TileMapLayer tilemap, Vector2 pos) => 
        tilemap.GetCellSourceId(tilemap.LocalToMap(pos)) != -1;

    static int GetCurrentTileId(this TileMapLayer tilemap, Vector2 pos)
    {
        Vector2I cellPos = tilemap.LocalToMap(pos);
        return 0;
        //return tilemap.GetCellv(cellPos); // TODO: Godot 4 conversion
    }
}
