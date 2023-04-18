namespace Template;

public static class Prefabs
{
    //public static PackedScene SomePrefab { get; } = Load("some_prefab");

    private static PackedScene Load(string path) =>
        GD.Load<PackedScene>($"res://Scenes/Prefabs/{path}.tscn");
}
