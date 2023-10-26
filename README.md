# Template
The template I use when starting a new Godot 4 C# game.

## Feature Summary
- Pre-configured [project.godot](https://github.com/ValksGodotTools/Template/blob/main/project.godot) [.csproj](https://github.com/ValksGodotTools/Template/blob/main/Template.csproj) [.editorconfig](https://github.com/ValksGodotTools/Template/blob/main/.editorconfig) [.gitignore](https://github.com/ValksGodotTools/Template/blob/main/.gitignore)
- [Godot Utils](https://github.com/ValksGodotTools/GodotUtils)
- [UIConsole](https://github.com/ValksGodotTools/Template/blob/main/Scripts/UI/Console/UIConsole.cs)
- [Hotkey Management](https://github.com/ValksGodotTools/Template/blob/main/Scripts/UI/Options/UIOptionsInput.cs)
- [Audio Management](https://github.com/ValksGodotTools/Template/blob/main/Scripts/Autoloads/AudioManager.cs)
- [Several Options](https://github.com/ValksGodotTools/Template/tree/main/Scripts/UI/Options)
- [Global Autoload](https://github.com/ValksGodotTools/Template/blob/main/Scripts/Autoloads/Global.cs)
- [Localisation](https://github.com/ValksGodotTools/Template/blob/main/Localisation/text.csv)
- [Credits Scene](https://github.com/ValksGodotTools/Template/blob/main/Scripts/UI/UICredits.cs)
- And lots more!

## Setup
1. Download and install the [latest Godot 4 C# release](https://godotengine.org/)
2. Clone this repository with all its submodules
```
git clone --recursive https://github.com/ValksGodotTools/Template
```

## Main Menu
![1](https://user-images.githubusercontent.com/6277739/236582661-9e7a67d6-cf01-4457-9162-b3edd76dd999.png)

## Options
![2](https://user-images.githubusercontent.com/6277739/236582663-34dc44b2-7c29-4acd-b3b6-5a733ac7988d.png)

## Credits
![3](https://user-images.githubusercontent.com/6277739/236582668-738667a7-3bf1-4074-b852-7735f1d57100.png)

## Hotkeys
![Untitled](https://user-images.githubusercontent.com/6277739/236582745-8d69b91f-497f-4188-b669-66daaa43691d.png)

## Services
**Stop using static everywhere!** Static exists for the lifetime of the application wasting valuable game memory. Instead lets make use of `Global.Services`.

In the `_Ready()` of any node add `Global.Services.Add(this)`. (if the script does not extend from node, you can use `Global.Services.Add<Type>`)
```cs
public partial class UIVignette : ColorRect
{
    public override void _Ready()
    {
        // Set persistent to true if this is an autoload script
        // (scripts that do not extend from Node are persistent by default)

        // Non persistent services will get removed just before the scene is changed
        // Example of persistent service: AudioManager; a node like this should exist
        // for the entire duration of the game

        // However this UIVignette exists within the scene so it should not be persistent
        Global.Services.Add(this, persistent: false); 
    }

    public void LightPulse() { ... }
}
```

Now you can get the instance of UIVignette from anywhere! No static or long GetNode\<T\> paths involved. It's magic.
```cs
UIVignette vignette = Global.Services.Get<UIVignette>();
vignette.LightPulse();
```

## Console Commands
```cs
// Simply add the "ConsoleCommand" attribute to any function
// it will be registered as a new console command

// Note to bring up the console in-game press F12

[ConsoleCommand("help")]
void Help()
{
    IEnumerable<string> cmds =
        Global.Services.Get<UIConsole>().Commands.Select(x => x.Name);

    Global.Services.Get<Logger>().Log(cmds.Print());
}

// Console commands can have aliases, this command has a
// alias called "exit"

[ConsoleCommand("quit", "exit")]
void Quit()
{
    GetTree().Root.GetNode<Global>("/root/Global").Quit();
}

// Method parameters are supported

[ConsoleCommand("debug")]
void Debug(int x, string y)
{
    Global.Services.Get<Logger>().Log($"Debug {x}, {y}");
}
```

## Prefabs
```cs
// Load all your scene prefabs here. This script can be found in
// "res://Scripts/Static/Prefabs.cs". Note that music and sounds are
// loaded in very similarily and these scripts can be found in the
// static folder as well.
public static class Prefabs
{
    public static PackedScene Options { get; } = Load("UI/options");

    static PackedScene Load(string path) =>
        GD.Load<PackedScene>($"res://Scenes/Prefabs/{path}.tscn");
}

// Prefabs are instantiated like this
UIOptions options = Prefabs.Options.Instantiate<UIOptions>();
```

## AudioManager
```cs
AudioManager audioManager = Global.Services.Get<AudioManager>();

// Play a soundtrack
audioManager.PlayMusic(Music.Menu);

// Play a sound
audioManager.PlaySFX(Sounds.GameOver);

// Set the music volume
audioManager.SetMusicVolume(75);

// Set the sound volume
audioManager.SetSFXVolume(100);

// Gradually fade out all sounds
audioManager.FadeOutSFX();
```

## SceneManager
```cs
// Switch to a scene instantly
Global.Services.Get<SceneManager>().SwitchScene("main_menu");

// Switch to a scene with a fade transition
Global.Services.Get<SceneManager>().SwitchScene("level_2D_top_down", 
    SceneManager.TransType.Fade);
```

## Experimental EventManager
This is __one__ way of programming events, it may not be the best way but there is no harm in trying it out. 

*In most tutorials about this on the internet you will find that they pass in a `Dictionary<string, string>` for the event params. This is very ugly but with the use of `dynamic` now you can pass in any kind of object you want and have beautiful params if you so desire.*

### Event Enums
```cs
public enum EventGeneric
{
    OnKeyboardInput
}

public enum EventPlayer
{
    OnPlayerSpawn
}
```

### Event Dictionaries
```cs
public static class Events
{
    public static EventManager<EventGeneric> Generic { get; } = new();
    public static EventManager<EventPlayer> Player { get; } = new();
}
```

### Example #1
```cs
Events.Generic.AddListener(EventGeneric.OnKeyboardInput, (args) => 
{
    GD.Print(args[0]);
    GD.Print(args[1]);
    GD.Print(args[2]);
}, "someId");

Events.Generic.RemoveListeners(EventGeneric.OnKeyboardInput, "someId");

// Listener is never called because it was removed
Events.Generic.Notify(EventGeneric.OnKeyboardInput, 1, 2, 3);
```

### Example #2
```cs
Events.Player.AddListener<PlayerSpawnArgs>(EventPlayer.OnPlayerSpawn, (args) => 
{
    GD.Print(args.Name);
    GD.Print(args.Location);
    GD.Print(args.Player);
});

Events.Player.Notify(EventPlayer.OnPlayerSpawn, new PlayerSpawnArgs(name, location, player));
```

## Contributing
Any kind of contributions are very much welcomed!

[Roadmap](https://github.com/ValksGodotTools/Template/issues/12)  

[Projects Coding Style](https://github.com/Valks-Games/sankari/wiki/Code-Style)  

Contact me over Discord (`valk2023`)

## Credit
See [credits.txt](https://github.com/ValksGodotTools/Template/blob/main/credits.txt)  
