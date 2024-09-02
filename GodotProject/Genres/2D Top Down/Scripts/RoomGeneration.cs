namespace Template;

public partial class RoomGeneration : Node
{
    [Export] TileMapLayer tileMap;

	public override void _Ready()
	{
        List<Vector2I> floorTiles = new();

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                string tileName = tileMap.GetCustomData<string>(new Vector2I(x, y), "name");

                if (tileName == "floor")
                {
                    floorTiles.Add(new Vector2I(x, y));
                }
            }
        }

        Random random = new();

        for (int i = 0; i < 2; i++)
        {
            Vector2 randomFloorPosition = tileMap.MapToLocal(floorTiles[random.Next(floorTiles.Count)]) * tileMap.Scale;

            Frog frog = Game.LoadPrefab<Frog>(Prefab.Frog);
            frog.Position = randomFloorPosition;

            AddChild(frog);
        }
    }
}
