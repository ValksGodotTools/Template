using Godot;

namespace Template;

public static class Music
{
    // All audio assets were removed from the Template to reduce bloated file size

    //public static AudioStream Menu { get; } =
    //    Load("SubspaceAudio/5 Chiptunes/Title Screen.wav");

    static AudioStream Load(string path)
    {
        return GD.Load<AudioStream>($"res://Audio/Songs/{path}");
    }
}

