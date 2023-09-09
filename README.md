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

## FAQ
Q: Why am I not seeing any messages appear in console?  

A: The AddMessage(...) function has been commented out 3 times in Logger.cs because what if a project is using GodotUtils submodule where the Logger.cs is defined but it's not using Template where UIConsole.Instance.AddMessage(...) is defined. I have not found a solution to this so for now please just uncomment the calls to AddMessage(...) in Logger.cs.  

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

<details>
  <summary>Old Video Previews</summary>
  
  <!--Spoiler text. Note that it's important to have a space after the summary tag. You should be able to write any markdown you want inside the `<details>` tag... just make sure you close `<details>` afterward.-->
  https://user-images.githubusercontent.com/6277739/233853220-cad1e927-8dcc-493d-a691-59e296c1b10f.mp4

  https://user-images.githubusercontent.com/6277739/234088697-11d94789-3a14-4aee-bc5b-ba8dee9f4461.mp4
  
</details>

## Console Commands
```cs
// Simply add the "ConsoleCommand" attribute to any function
// it will be registered as a new console command

// Note to bring up the console in-game press F12

[ConsoleCommand("help")]
void Help()
{
    IEnumerable<string> cmds =
        UIConsole.Instance.Commands.Select(x => x.Name);

    Logger.Log(cmds.Print());
}

// Console commands can have aliases, this command has a
// alias called "exit"

[ConsoleCommand("quit", "exit")]
void Quit()
{
    GetTree().Root.GetNode<Global>("/root/Global").Quit();
}

[ConsoleCommand("debug")]
void Debug()
{
    Logger.Log("Debug");
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
// Play a soundtrack
AudioManager.Instance.PlayMusic(Music.Menu);

// Play a sound
AudioManager.Instance.PlaySFX(Sounds.GameOver);

// Set the music volume
AudioManager.Instance.SetMusicVolume(75);

// Set the sound volume
AudioManager.Instance.SetSFXVolume(100);

// Gradually fade out all sounds
AudioManager.Instance.FadeOutSFX();
```

## SceneManager
```cs
// Switch to a scene instantly
SceneManager.Instance.SwitchScene("main_menu");

// Switch to a scene with a fade transition
SceneManager.Instance.SwitchScene("level_2D_top_down", 
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

[Projects Coding Style](https://github.com/Valks-Games/sankari/wiki/Code-Style)

If you have any questions, talk to me over Discord (`valk2023`)
