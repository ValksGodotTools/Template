namespace Template;

public static class Prefabs
{
    //public static PackedScene SomePrefab { get; } = LoadPrefab("some_prefab");

    private static PackedScene LoadPrefab(string path) =>
        GD.Load<PackedScene>($"res://Scenes/Prefabs/{path}.tscn");
}
