namespace Template;

public static class Music
{
    //public static AudioStream Menu { get; } =
    //    Load("SubspaceAudio/5 Chiptunes/Title Screen.wav");

    static AudioStream Load(string path) =>
        GD.Load<AudioStream>($"res://Audio/Songs/{path}");
}
