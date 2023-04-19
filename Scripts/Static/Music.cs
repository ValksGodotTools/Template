namespace Template;

public static class Music
{
    public static AudioStream Menu { get; } = 
        Load("SubspaceAudio/5 Chiptunes/Title Screen.wav");

    public static AudioStream Level1 { get; } =
        Load("SubspaceAudio/5 Chiptunes/Level 1.wav");

    public static AudioStream Level2 { get; } =
        Load("SubspaceAudio/5 Chiptunes/Level 2.wav");

    public static AudioStream Level3 { get; } =
        Load("SubspaceAudio/5 Chiptunes/Level 3.wav");

    public static AudioStream Level4 { get; } =
        Load("SubspaceAudio/5 Chiptunes/Ending.wav");

    private static AudioStream Load(string path) =>
        GD.Load<AudioStream>($"res://Audio/Songs/{path}");
}
