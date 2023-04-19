namespace Template;

public partial class ResourceOptions : Resource
{
    // Resource props must have [Export] attribute otherwise
    // they will not save properly
    [Export] public float MusicVolume { get; set; } = 100;
    [Export] public float SFXVolume   { get; set; } = 100;
}
