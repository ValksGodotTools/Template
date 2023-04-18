namespace Template;

public partial class UIMainMenu : Node
{
    public override void _Ready()
    {
        var musicPlayer = GetNode<AudioStreamPlayer>("Music");
        musicPlayer.Play();

        // There is a bug in Godot when using += operator so that is why we have
        // to use .Connect(...) for now until https://github.com/godotengine/godot/issues/70414
        // is fixed.
        // Furthermore, Godot signals should be used over C# events because Godot
        // signals handle unsubscribe events automatically for us.
        // Note that an alternative to AudioManager.Instance.Connect(...) is
        // GetNode<AudioManager>("/root/AudioManager").Connect(...)
        AudioManager.Instance.Connect(AudioManager.SignalName.VolumeMusicChanged, 
            Callable.From<float>((v) =>
            {
                musicPlayer.VolumeDb = v <= -40 ? -80 : v;
            }));
    }

    private void _on_play_pressed()
    {
        SceneManager.SwitchScene("level", SceneManager.TransType.Fade);
    }

    private void _on_options_pressed()
    {
        SceneManager.SwitchScene("options");
    }

    private void _on_quit_pressed()
    {
        Global.Quit();
    }
}
