# Template
I was tired of doing the same things over and over again for each new game I started working on. So that's what this template is for, something that I can use in all my games as a starting point.

https://user-images.githubusercontent.com/6277739/233207169-180570a5-4c9a-4ab7-8ea6-87b3febd7e86.mp4

## Features
- Pre-configured [project.godot](https://github.com/ValksGodotTools/Template/blob/main/project.godot) [.csproj](https://github.com/ValksGodotTools/Template/blob/main/Template.csproj) [.editorconfig](https://github.com/ValksGodotTools/Template/blob/main/.editorconfig) [.gitignore](https://github.com/ValksGodotTools/Template/blob/main/.gitignore)
- [Godot Utils](https://github.com/ValksGodotTools/GodotUtils)
- [Audio Management](https://github.com/ValksGodotTools/Template/blob/main/Scripts/Autoloads/AudioManager.cs)
- [Several Options](https://github.com/ValksGodotTools/Template/blob/main/Scripts/UI/UIOptions.cs)
- [Global Autoload](https://github.com/ValksGodotTools/Template/blob/main/Scripts/Autoloads/Global.cs)
- [Localisation](https://github.com/ValksGodotTools/Template/blob/main/Localisation/text.csv)
- [Credits Scene](https://github.com/ValksGodotTools/Template/blob/main/Scripts/UI/UICredits.cs)

## Roadmap
- Add 3 different types of level scenes; 2D Top down, 2D Platformer, 3D FPS (each scene will have basic player controllers setup)
- Hotkey management
- ModLoader logic and UI

## Known Issues
- Borderless mode does not fully cover the entire screen (there is a 2 pixel border gap all around)
- Window Size option is sometimes not displaying the correct width and height values

## Setup
1. Download and install the [latest Godot 4 C# release](https://godotengine.org/)
2. Clone this repository with all its submodules
```
git clone --recursive https://github.com/ValksGodotTools/Template
```

## Contributing
Please have a look at the [Projects Coding Style](https://github.com/Valks-Games/sankari/wiki/Code-Style).

I highly recommend you install the [GitHub Desktop App](https://desktop.github.com/). This will almost completely eliminate the need to do Git CLI commands through the command line.

If you are interested in contributing or have any questions please talk to `va#9904` over Discord.

## Credit
See [credits.txt](https://github.com/ValksGodotTools/Template/blob/main/credits.txt)
