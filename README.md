# Template
A template for all Godot 4 C# starting projects. See the video below for a showcase of what's here so far.

https://user-images.githubusercontent.com/6277739/233853220-cad1e927-8dcc-493d-a691-59e296c1b10f.mp4

## Features
- Pre-configured [project.godot](https://github.com/ValksGodotTools/Template/blob/main/project.godot) [.csproj](https://github.com/ValksGodotTools/Template/blob/main/Template.csproj) [.editorconfig](https://github.com/ValksGodotTools/Template/blob/main/.editorconfig) [.gitignore](https://github.com/ValksGodotTools/Template/blob/main/.gitignore)
- [Godot Utils](https://github.com/ValksGodotTools/GodotUtils)
- [UIConsole](https://github.com/ValksGodotTools/GodotUtils/blob/main/Scripts/Console/UIConsole.cs)
- [Hotkey Management](https://github.com/ValksGodotTools/Template/blob/main/Scripts/UI/UIHotkeys.cs)
- [Audio Management](https://github.com/ValksGodotTools/Template/blob/main/Scripts/Autoloads/AudioManager.cs)
- [Several Options](https://github.com/ValksGodotTools/Template/blob/main/Scripts/UI/UIOptions.cs)
- [Global Autoload](https://github.com/ValksGodotTools/Template/blob/main/Scripts/Autoloads/Global.cs)
- [Localisation](https://github.com/ValksGodotTools/Template/blob/main/Localisation/text.csv)
- [Credits Scene](https://github.com/ValksGodotTools/Template/blob/main/Scripts/UI/UICredits.cs)

## Roadmap
- Add 3 different types of level scenes; 2D Top down, 2D Platformer, 3D FPS (each scene will have basic player controllers setup)

## Known Issues
(1) Borderless mode does not fully cover the entire screen (there is a 2 pixel border gap all around)  
(2) Pressing escape while in level.tscn while the hotkeys is open and currently waiting for new hotkey, escapes out of the hotkey menu back to the popup menu. This behaviour is undesired. Instead the first escape should cancel the waiting for hotkey input. Then the next escape should exit out of the options menu.  

## Setup
1. Download and install the [latest Godot 4 C# release](https://godotengine.org/)
2. Clone this repository with all its submodules
```
git clone --recursive https://github.com/ValksGodotTools/Template
```

## Contributing
Currently looking for programmers to peer review my code. I am struggling trying to figure out how to solve known issue (2).

[Projects Coding Style](https://github.com/Valks-Games/sankari/wiki/Code-Style)

If you have any questions, talk to me over Discord (`va#9904`)

## Credit
See [credits.txt](https://github.com/ValksGodotTools/Template/blob/main/credits.txt)
