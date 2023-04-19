namespace Template;

using VSyncMode = DisplayServer.VSyncMode;

/* 
 * If the ResourceOptions.cs script is moved then the file path will not updated
 * in the .tres file. In order to fix this go to 
 * C:\Users\VALK-DESKTOP\AppData\Roaming\Godot\app_userdata\Template
 * and delete the .tres file so Godot will be forced to generate it from
 * scratch. This is not a Godot bug it is just something to look out for.
 * 
 * Resource props must have [Export] attribute otherwise they will not save 
 * properly.
 */
public partial class ResourceOptions : Resource
{
    [Export] public float      MusicVolume { get; set; } = 100;
    [Export] public float      SFXVolume   { get; set; } = 100;
    [Export] public WindowMode WindowMode  { get; set; } = WindowMode.Windowed;
    [Export] public VSyncMode  VSyncMode   { get; set; } = VSyncMode.Enabled;
    [Export] public Vector2I   WindowSize  { get; set; }
    [Export] public int        MaxFPS      { get; set; } = 60;
    [Export] public Language   Language    { get; set; } = Language.English;
}
