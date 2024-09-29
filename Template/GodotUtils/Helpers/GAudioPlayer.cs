using Godot;

namespace GodotUtils;

public class GAudioPlayer
{
    /// <summary>
    /// <para>
    /// Set the volume from a value of 0 to 100
    /// </para>
    /// 
    /// <para>
    /// The value will be auto converted to values
    /// Godot can work with
    /// </para>
    /// </summary>
    public float Volume
    {
        get => StreamPlayer.VolumeDb.Remap(-40, 0, 0, 100);
        set
        {
            float v = value.Remap(0, 100, -40, 0);

            if (value == 0)
            {
                v = -80;
            }

            StreamPlayer.VolumeDb = v;
        }
    }

    public bool Playing
    {
        get => StreamPlayer.Playing;
        set => StreamPlayer.Playing = value;
    }

    public AudioStream Stream
    {
        get => StreamPlayer.Stream;
        set => StreamPlayer.Stream = value;
    }

    public float Pitch
    {
        get => StreamPlayer.PitchScale;
        set => StreamPlayer.PitchScale = value;
    }

    public AudioStreamPlayer StreamPlayer { get; }

    public GAudioPlayer(Node parent, bool deleteOnFinished = false)
    {
        StreamPlayer = new AudioStreamPlayer();

        if (deleteOnFinished)
        {
            StreamPlayer.Finished += () => StreamPlayer.QueueFree();
        }

        parent.AddChild(StreamPlayer);
    }

    public void Play()
    {
        StreamPlayer.Play();
    }
}

