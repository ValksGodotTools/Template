# Template
A template for all Godot 4 C# starting projects.

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

## Contributing
Currently looking for programmers to peer review my code.

[Projects Coding Style](https://github.com/Valks-Games/sankari/wiki/Code-Style)

If you have any questions, talk to me over Discord (`valk2023`)
