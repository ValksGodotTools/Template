# Template
Never again spend 5 minutes setting up a new project, ValksGodotTools/Template has got your back. ❤️

## Feature Summary
### Useful Functions
[Godot Utils](https://github.com/ValksGodotTools/GodotUtils) contains several useful extensions such as `.PrintFull()`, `.ForEach()` and `.QueueFreeChildren()`.

There are also useful classes like [Scene Manager](#scenemanager) and [Services](#services) that make your life easier.

### Key Bindings
Full configuration of [key bindings](#hotkeys) in-game.

### In-Game Console
Press `F12` to bring up the [console](#console-commands) in game. New commands are very easy to code in.

### Localisation
Add in your own [languages](https://github.com/ValksGodotTools/Template/blob/main/Localisation/text.csv).

### Mod Loader
[Mods](#mod-loader) can replace game assets and execute C# scripts.

## Setup
> Please note if you used this project before commit 45a5b58 ("Separated Template files from game files") and you are pulling the latest changes you will need to delete the following folder `C:\Users\YOUR_USERNAME_HERE\AppData\Roaming\Godot\app_userdata\Template`

1. Download and install the [latest Godot 4 C# release](https://godotengine.org/)
2. Clone this repository with all its submodules
```
git clone --recursive https://github.com/ValksGodotTools/Template
```

If for whatever reason you forget to clone with the `--recursive` flag. Run the following command below to retrieve the submodules.
```
git submodule update --init --recursive
```

## Main Menu
![1](https://user-images.githubusercontent.com/6277739/236582661-9e7a67d6-cf01-4457-9162-b3edd76dd999.png)

## Options
![2](https://user-images.githubusercontent.com/6277739/236582663-34dc44b2-7c29-4acd-b3b6-5a733ac7988d.png)

## Credits
![3](https://user-images.githubusercontent.com/6277739/236582668-738667a7-3bf1-4074-b852-7735f1d57100.png)

## Hotkeys
![Untitled](https://user-images.githubusercontent.com/6277739/236582745-8d69b91f-497f-4188-b669-66daaa43691d.png)

## Mod Loader
You can find the example mod repo [here](https://github.com/ValksGodotTools/ExampleMod)

![Untitled](https://github.com/ValksGodotTools/Template/assets/6277739/8c360277-7da0-4cd6-8b10-24c6cfe084db)

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
If you like the idea of having a universal static event manager that handles everything then try out the code below in your own project.

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

Contact me over Discord (`valky5`)

## Credit
See [credits.txt](https://github.com/ValksGodotTools/Template/blob/main/credits.txt)  
