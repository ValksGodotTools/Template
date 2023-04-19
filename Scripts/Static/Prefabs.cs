namespace Template;

public static class Prefabs
{
    public static PackedScene Options { get; } = Load("options");

    private static PackedScene Load(string path) =>
        GD.Load<PackedScene>($"res://Scenes/Prefabs/{path}.tscn");
}
