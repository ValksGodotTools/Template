using Godot;

namespace Template;

public static class Sounds
{
    public static AudioStream GameOver { get; } =
        Load("Game Over/musical-game-over.wav");

    private static AudioStream Load(string path)
    {
        return GD.Load<AudioStream>($"res://Audio/SFX/{path}");
    }
}

