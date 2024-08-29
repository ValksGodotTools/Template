# Godot 4 C# Template
Never again spend 5 minutes setting up a new project, `ValksGodotTools/Template` has got your back. ❤️

Want to get right into it? Start off by reading the [setup guide](#setup-guide).

1. [Setup Guide](#setup-guide)
2. [Features](#features)
    - [Multiplayer](#multiplayer)
    - [FPS Scene](#fps-scene)
    - [Mod Loader](#mod-loader)
    - [Godot Utils](#godot-utils)
    - [Localisation](#localisation)
    - [Services](#services)
    - [Console Commands](#console-commands)
    - [Audio Manager](#audiomanager)
    - [Scene Manager](#scenemanager)
    - [State Manager](#state-manager)
    - [Experimental Event Manager](#experimental-eventmanager)
3. [Tips](#tips)
4. [Contributing](#contributing)
5. [Roadmap](#roadmap)
6. [Credits](#credits)

![main-menu](https://github.com/ValksGodotTools/Template/assets/6277739/e8abf19d-0ac7-4ae3-9942-e1b406edf7cf)  
![options](https://github.com/ValksGodotTools/Template/assets/6277739/c5a9e011-f433-4887-8947-36130dd83426)  
![keybindings](https://user-images.githubusercontent.com/6277739/236582745-8d69b91f-497f-4188-b669-66daaa43691d.png)  

## Setup Guide

### 🌐 Download the repo
1. Download and install the [latest Godot 4 C# release](https://godotengine.org/)
2. Clone with `git clone --recursive https://github.com/ValksGodotTools/Template`

*If the GodotUtils folder is still empty, run `git submodule update --init --recursive`*

### 🔧 Run the game with `F5`
Fill in the fields and click `Apply`. This will close the game.

![setup-scene](https://github.com/user-attachments/assets/00262157-26e1-4909-9a71-7a3357a8c126)  

### 🚀 Press `F5` one more time!

You are done! If you selected the 3D FPS genre then the 3D FPS scene will load!

> [!IMPORTANT]
> A internet connection is required when running the game with `F5` for the first time. This is because the `.csproj` needs to retrieve the NuGet packages from the NuGet website.

## Features
### Multiplayer
The 2D Top Down genre has a **client authorative** multiplayer setup for showing players positions updating on each others screens. This netcode is the result of redoing the same multiplayer project over and over again. I've lost track how many times I've done this now. I hope you will find the multiplayer as useful as I have.

https://github.com/user-attachments/assets/964ced37-4a20-4de8-87ee-550fe5ecb561

> [!IMPORTANT]
> A very common mistake is to write one data type and read another data type. For example lets say you have the integer `playerCount` and you do `writer.Write(playerCount)` and then `playerCount = reader.ReadByte()`. Since you did not cast playerCount to byte on writing, you will receive malformed data. Lets always cast our data values before writing them even if it may seem redundant at times.

> [!CAUTION]
> Do not directly access properties or methods across threads unless they are explicity marked as thread safe. Not following thread safety will result in random crashes with no errors logged to the console. If you want to avoid logs getting jumbled use `Game.Log(...)` over `GD.Print(...)`.

Here is what a client packet could look like. The client is using this packet to tell the server its position. The `Handle(...)` is executed on the server thread so only things on that thread should be accessed.
```cs
public class CPacketPosition : ClientPacket
{
    public Vector2 Position { get; set; }

    public override void Write(PacketWriter writer)
    {
        writer.Write((Vector2)Position);
    }

    public override void Read(PacketReader reader)
    {
        Position = reader.ReadVector2();
    }

    public override void Handle(ENetServer s, Peer client)
    {
        GameServer server = (GameServer)s;
        server.Players[client.ID].Position = Position;
    }
}
```

Here is what a server packet could look like. The server is telling each client about all the others client position updates. The `Handle(...)` is executed on the client thread so only things on that thread should be accessed.
```cs
public class SPacketPlayerPositions : ServerPacket
{
    public Dictionary<uint, Vector2> Positions { get; set; }

    public override void Write(PacketWriter writer)
    {
        writer.Write((byte)Positions.Count);

        foreach (KeyValuePair<uint, Vector2> pair in Positions)
        {
            writer.Write((uint)pair.Key);
            writer.Write((Vector2)pair.Value);
        }
    }

    public override void Read(PacketReader reader)
    {
        Positions = new();

        byte count = reader.ReadByte();

        for (int i = 0; i < count; i++)
        {
            uint id = reader.ReadUInt();
            Vector2 position = reader.ReadVector2();

            Positions.Add(id, position);
        }
    }

    public override void Handle(ENetClient client)
    {
        Level level = Global.Services.Get<Level>();

        foreach (KeyValuePair <uint, Vector2> pair in Positions)
        {
            if (level.OtherPlayers.ContainsKey(pair.Key))
                level.OtherPlayers[pair.Key].LastServerPosition = pair.Value;
        }

        // Send a client position packet to the server immediately right after
        // a server positions packet is received
        level.Player.NetSendPosition();
    }
}
```

Sending a packet from the client
```cs
// Player.cs
Net net = Global.Services.Get<Net>();

net.Client.Send(new CPacketPosition
{
    Position = Position
});
```

Sending a packet from the server
```cs
Send(new SPacketPlayerPositions
{
    Positions = GetOtherPlayers(pair.Key).ToDictionary(x => x.Key, x => x.Value.Position)
}, Peers[pair.Key]);
```

### FPS Scene

https://github.com/user-attachments/assets/db2dea51-25be-4714-9476-a061135c44ac

> [!NOTE]
> All animations were made by myself from Blender. You are free to use them in your game.

![Untitled](https://github.com/user-attachments/assets/7f5395cd-2ac6-46a6-a386-2c665aff98aa)

> [!TIP]
> Are you tired of weird rotational glitches? Quaternions are your friend! Every `Node3D` has a `.Quaternion` property. Quaternions are multiplied together and always normalized. For example `(A * B * C).Normalized()`. The order of which you multiply quaternions matters! This is what I used to get the weapon camera movements working.

### Mod Loader
> [!NOTE]
> Mods can replace game assets and execute C# scripts. There is currently a big problem with C# mod scripts, see https://github.com/ValksGodotTools/Template/issues/15. You can find the example mod repository [here](https://github.com/ValksGodotTools/ExampleMod).

### Godot Utils
The submodule [Godot Utils](https://github.com/ValksGodotTools/GodotUtils) contains useful classes and extensions.

#### Creating Tweens
Tweening has never been so easy :fire:
```cs
new GTween(colorRect)
    .SetParallel()
    .Animate("scale", Vector2.One * 2, 2).Elastic()
    .Animate("color", Colors.Green, 2).Sine().EaseIn()
    .Animate("rotation", Mathf.Pi, 2).Elastic().EaseOut();

GTween tween = new GTween(colorRect)
    .SetAnimatingProp("color")
    .AnimateProp(Colors.Red, 0.5).Sine().EaseIn()
    .Parallel().AnimateProp(Colors.Green, 0.5).Sine().EaseOut()
    .Parallel().Animate("scale", Vector2.One * 2, 0.5).Sine()
    .Callback(() => GD.Print("Finished!"))
    .Loop();

tween.Stop();
```

> [!TIP]
> Use tweens to execute delayed code. Tweens are attached to nodes so if the node gets destroyed so will the tween.
> ```cs
> GTween.Delay(node, seconds, () => callback);
> ```

#### Highlighted Functions
```cs
// Prints all public properties and fields from this node
GD.Print(node.PrintFull());

// Recursively searches the nodes children for node of type T
T node = node.GetNode<T>();

// Thread safe logger
Game.Log()

// Fetches the first animation frame and crops it to only non-transparent pixels
animatedSprite.GetScaledSize();

// Useful shortcut extensions
animationTree.SetCondition()
animationTree.SetParam()

// Other useful functions
node.QueueFreeChildren();
node.AddChildDeferred();
node.WaitOneFrame();
IEnumerable<T>.ForEach()
lineEdit.Filter()
GU.GetLayerValues()
GU.LoadPrefab<T>()

// AND MANY MORE!!!
```

### Localisation
> [!NOTE]
> Currently English, French and Japanese are supported for most of the UI elements. You can add in your own languages [here](https://github.com/ValksGodotTools/Template/blob/main/Localisation/text.csv).

### Services
> [!IMPORTANT]
> In order to understand how useful `Global.Services` is, let me tell you why using the static keyword should be avoided. Lets say you are coding a multiplayer game and you make every property in `GameServer.cs` static. Everything works fine at first and you can easily access the game servers properties from *almost* anywhere but once you restart the server or leave the scene where the game server shouldn't be alive anymore, the old values for each static property will still exist from the last time the server was online. You would have to keep track of each individual property you made static and reset them. This is why static should be avoided.

In the `_Ready()` of any node add `Global.Services.Add(this)` *(if the script does not extend from node, you can use `Global.Services.Add<Type>`)*
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

### Console Commands
Adding the `ConsoleCommand` attribute to any function will register it as a new console command.

> [!NOTE]
> The in-game console can be brought up with `F12`

```cs
[ConsoleCommand("help")]
void Help()
{
    IEnumerable<string> cmds =
        Global.Services.Get<UIConsole>().Commands.Select(x => x.Name);

    Game.Log(cmds.Print());
}
```

Console commands can have aliases, this command has an alias named "exit"
```cs
[ConsoleCommand("quit", "exit")]
void Quit()
{
    GetTree().Root.GetNode<Global>("/root/Global").Quit();
}
```

Method parameters are supported
```cs
[ConsoleCommand("debug")]
void Debug(int x, string y)
{
    Game.Log($"Debug {x}, {y}");
}
```

### AudioManager
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

### SceneManager
```cs
// Switch to a scene instantly
Global.Services.Get<SceneManager>().SwitchScene("main_menu");

// Switch to a scene with a fade transition
Global.Services.Get<SceneManager>().SwitchScene("level_2D_top_down", 
    SceneManager.TransType.Fade);
```

### State Manager
This state manager uses **functions as states** as suppose to using classes for states. The [`State`](https://github.com/ValksGodotTools/GodotUtils/blob/ccd37342ab8d758a664d2abd3375a21b608d2198/State.cs) class is provided in the GodotUtils submodule. Below an example is given.

Create a new file named `Player.cs` and add the following script to it.
```cs
public partial class Player : Entity // This script extends from Entity but it may extend from CharacterBody3D for you
{
    State curState;

    public override void _Ready()
    {
        curState = Idle();
        curState.Enter();
    }

    public override void _PhysicsProcess(double delta)
    {
        curState.Update(delta);
    }

    public void SwitchState(State newState)
    {
        GD.Print($"Switched from {curState} to {newState}"); // Useful for debugging. May be more appealing to just say "Switched to {newState}" instead.

        curState.Exit();
        newState.Enter();
        curState = newState;
    }
}
```
Create another file named `PlayerIdle.cs` and add the following.
```cs
public partial class Player
{
    State Idle()
    {
        var state = new State(this, nameof(Idle));

        state.Enter = () =>
        {
            // What happens on entering the idle state?
        };

        state.Update = delta =>
        {
            // What happens on every frame in the idle state?
        };

        state.Exit = () =>
        {
            // What happens on exiting the idle state?
        }

        return state;
    }
}
```
Do a similar process when adding new states.

### Experimental EventManager
If you like the idea of having a universal static event manager that handles everything then try out the code below in your own project.

#### Event Enums
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

#### Event Dictionaries
```cs
public static class Events
{
    public static EventManager<EventGeneric> Generic { get; } = new();
    public static EventManager<EventPlayer> Player { get; } = new();
}
```

#### Example #1
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

#### Example #2
```cs
Events.Player.AddListener<PlayerSpawnArgs>(EventPlayer.OnPlayerSpawn, (args) => 
{
    GD.Print(args.Name);
    GD.Print(args.Location);
    GD.Print(args.Player);
});

Events.Player.Notify(EventPlayer.OnPlayerSpawn, new PlayerSpawnArgs(name, location, player));
```

## Tips
> [!TIP]
> If you need to execute code before the game quits you can listen to OnQuit.
> ```cs
> // This is an async function because you way want to await certain processes before the game exists
> Global.Services.Get<Global>().OnQuit += async () =>
> {
>     // Execute your code here
>     await Task.FromResult(1);
> }
> ```

> [!CAUTION]
> Avoid deleting `res://Template` and `res://GodotUtils`, doing so will cause certain features to stop working. I have tried my best to move all assets you would need to modify for your game outside of `res://Template` into `res://`. If you want to modify the contents of `res://GodotUtils`, please consider creating a pull request on the [repo](https://github.com/ValksGodotTools/GodotUtils) first.

## Contributing
> [!IMPORTANT]
> Please have a quick look at the [Projects Coding Style](https://github.com/Valks-Games/sankari/wiki/Code-Style) and contact me over Discord before contributing. My Discord username is `valky5`.

> [!NOTE]
> Here are some [good first issues](https://github.com/ValksGodotTools/Template/issues?q=is%3Aissue+is%3Aopen+label%3A%22good+first+issue%22) to tackle.

## Roadmap
> [!NOTE]
> The roadmap has been moved to [my Discord](https://discord.gg/5e7FkWCMRb). I find it easier to edit it here.

## Credits
> [!NOTE]
> For all credit to in-game assets used, see [credits.txt](https://github.com/ValksGodotTools/Template/blob/main/credits.txt).  
