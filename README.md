# Godot 4 C# Template
Say goodbye to the hassle of setting up a new project. `ValksGodotTools/Template` is here to streamline your workflow. ❤️

Ready to dive in? Check out the [setup guide](#setup-guide).

1. [Setup Guide](#setup-guide)
2. [Features](#features)
    - [Multiplayer](#multiplayer)
    - [FPS Scene](#fps-scene)
    - [Godot Utils](#godot-utils)
    - [Loading Prefabs](#loading-prefabs)
    - [Localisation](#localisation)
    - [Services](#services)
    - [Console Commands](#console-commands)
    - [Audio Manager](#audiomanager)
    - [Scene Manager](#scenemanager)
    - [State Manager](#state-manager)
    - [Mod Loader](#mod-loader)
3. [Tips](#tips)
4. [FAQ](#faq)
5. [Contributing](#contributing)
6. [Credits](#credits)

## Setup Guide

### 🌐 Download the repo
1. Download and install the [latest Godot 4 C# release](https://godotengine.org/)
2. Clone the repository using `git clone --recursive https://github.com/ValksGodotTools/Template`
3. Open `project.godot` located in `Template/GodotProject/project.godot`

### 🔧 Run the game with `F5`

Fill in the required fields and click `Apply`. This will close the game.

![setup-scene](https://github.com/user-attachments/assets/ee2adf8a-56dc-4a6f-9db6-ddb7f74f1e56)

### 🚀 Almost there!

1. Do not save anything if prompted, close the Godot editor entirely
2. Reopen the project and run the new main scene by pressing `F5`
3. If you chose the 3D FPS genre then the 3D FPS scene will load!

> [!IMPORTANT]
> If you encounter any issues, please refer to the [FAQ](#faq) before creating a new issue

![main-menu](https://github.com/ValksGodotTools/Template/assets/6277739/e8abf19d-0ac7-4ae3-9942-e1b406edf7cf)  
![options](https://github.com/ValksGodotTools/Template/assets/6277739/c5a9e011-f433-4887-8947-36130dd83426)  
![keybindings](https://user-images.githubusercontent.com/6277739/236582745-8d69b91f-497f-4188-b669-66daaa43691d.png)  

## Features
### Multiplayer
The 2D Top Down genre includes a **client-authoritative** multiplayer setup, demonstrating how player positions update on each other's screens. This netcode is the culmination of numerous iterations on multiplayer projects. I've lost count of how many times I've done this.

https://github.com/user-attachments/assets/964ced37-4a20-4de8-87ee-550fe5ecb561

#### First Look at a Client Packet
Below is an example of a client packet. The client uses this packet to inform the server of its position. The `Handle(...)` method is executed on the server thread, so only elements accessible on that thread should be accessed.

> [!IMPORTANT]
> Do not directly access properties or methods across threads unless they are explicity marked as thread safe. Not following thread safety will result in random crashes with no errors logged to the console.

```cs
public class CPacketPosition : ClientPacket
{
    [NetSend(1)]
    public Vector2 Position { get; set; }

    public override void Handle(ENetServer s, Peer client)
    {
        // The packet handled server-side (ENet Server thread)
    }
}
```

#### First Look at a Server Packet
Below is an example of a server packet. The server uses this packet to inform each client about the position updates of all other clients. The `Handle(...)` method is executed on the client thread, so only elements accessible on that thread should be accessed.
```cs
public class SPacketPlayerPositions : ServerPacket
{
    [NetSend(1)]
    public Dictionary<uint, Vector2> Positions { get; set; }

    public override void Handle(ENetClient client)
    {
        // The packet handled client-side (Godot thread)
    }
}
```

#### Net Send Attribute
This client packet sends the username then the position in this order.
```cs
public class CPacketJoin : ClientPacket
{
    [NetSend(1)]
    public string Username { get; set; }

    [NetSend(2)]
    public Vector2 Position { get; set; }

    public override void Handle(ENetClient client)
    {
        // The packet handled client-side (Godot thread)
    }
}
```

#### Handling Conditional Logic
Do not use the NetSend attribute if you need to use conditional logic.

> [!IMPORTANT]
> A common oversight is using one data type for writing and another for reading. For example, if you have an integer `playerCount` and you write it with `writer.Write(playerCount)`, but then read it as a byte with `playerCount = reader.ReadByte()`, the data will be malformed because `playerCount` wasn't converted to a byte prior to writing. To avoid this, ensure you cast your data to the correct type before writing, even if it feels redundant.

```cs
public class SPacketPlayerJoinLeave : ServerPacket
{
    public uint Id { get; set; }
    public string Username { get; set; }
    public Vector2 Position { get; set; }
    public bool Joined { get; set; }

    public override void Write(PacketWriter writer)
    {
        writer.Write((uint)Id);
        writer.Write((bool)Joined);

        if (Joined)
        {
            writer.Write((string)Username);
            writer.Write((Vector2)Position);
        }
    }

    public override void Read(PacketReader reader)
    {
        Id = reader.ReadUInt();

        Joined = reader.ReadBool();

        if (Joined)
        {
            Username = reader.ReadString();
            Position = reader.ReadVector2();
        }
    }

    public override void Handle(ENetClient client)
    {
        // The packet handled client-side (Godot thread)
    }
}
```

#### Sending a Packet from the Client
```cs
// Player.cs
Net net = Global.Services.Get<Net>();

net.Client.Send(new CPacketPosition
{
    Position = Position
});
```

#### Sending a Packet from the Server
```cs
Send(new SPacketPlayerPositions
{
    Positions = GetOtherPlayers(pair.Key).ToDictionary(x => x.Key, x => x.Value.Position)
}, Peers[pair.Key]);
```

#### Net Exclude Attribute
Using the `[NetExclude]` attribute will exclude properties from being written or read in the network.
```cs
public class PlayerData
{
    public string Username { get; set; }
    public Vector2 Position { get; set; }

    [NetExclude] 
    public Vector2 PrevPosition { get; set; }
}
```

### FPS Scene

https://github.com/user-attachments/assets/db2dea51-25be-4714-9476-a061135c44ac

> [!NOTE]
> All animations were made by myself from Blender. You are free to use them in your game.

![Untitled](https://github.com/user-attachments/assets/7f5395cd-2ac6-46a6-a386-2c665aff98aa)

> [!TIP]
> Tired of strange rotational issues? Quaternions can be your ally! Every `Node3D` has a `.Quaternion` property. Quaternions are combined by multiplication and are always normalized, like `(A * B * C).Normalized()`. Remember, the order in which you multiply quaternions is significant! This technique helped me achieve smooth weapon camera movements.

### Godot Utils

#### 🦄 Creating Tweens
Tweening has never been so easy!
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
> Below is an example of how to run delayed code. Tweens are attached to nodes so if the node gets destroyed so will the tween.
> ```cs
> GTween.Delay(node, seconds, () => callback);
> ```

#### 🖨️ Printing Everything
The `.PrintFull()` extension method will output all public properties and fields of any object, including nodes!

```cs
GD.Print(node.PrintFull())
```

#### 🔎 Finding Node\<T\>
If you have an array of entity nodes and each entity contains a Sprite2D node somewhere in its hierarchy, but you're not sure where exactly, you can use `entity.GetNode<Sprite2D>()` to find it. This function searches through the children recursively until it finds the first matching type. Keep in mind, this function can be somewhat resource-intensive, so consider using it only when necessary.

#### 🦆 Thread Safe Logger
By using `Game.Log()`, you can ensure that your logs are consistent across any thread. This means you won't have to deal with mixed-up logs when logging from the client, server, or Godot threads.

### Loading Prefabs
All prefabs (`.tscn` files) are loaded from `res://Scenes/Prefabs` and each one generates a `Prefab` enum entry when you build the project. Upon making changes to the prefabs, you will need to restart your IDE to see the updated `Prefab` enum.

If a prefab is located in for example `res://Scenes/Prefabs/Enemy/snowman.tscn` then `Prefab.EnemySnowman` will be created.

Prefabs are loaded using the following syntax:

```cs
// This assumes there is a Player.cs script attached to the Player node
Player player = Game.LoadPrefab<Player>(Prefab.Player);
```

### Localisation
> [!NOTE]
> By using `Game.Log()`, you can ensure that your logs are consistent across any thread. This means you won't have to deal with mixed-up logs when logging from the client, server, and Godot threads.

### Services
> [!IMPORTANT]
> To grasp the significance of `Global.Services`, it's essential to understand the pitfalls of using the `static` keyword. Consider a scenario where you're developing a multiplayer game and you designate every attribute in `GameServer.cs` as static. Initially, everything operates smoothly, and you can conveniently access the game server's properties from various parts of your code. However, challenges arise when the server restarts or when the scene transitions, and the game server should no longer be active. In such cases, the static properties retain their previous values, leading to inconsistencies. You would need to meticulously monitor and reset each static property, which can be cumbersome and error-prone. This illustrates why the use of static properties should be approached with caution.

In the `_Ready()` method of any node, you can register the node with `Global.Services` by using `Global.Services.Add(this)` (or `Global.Services.Add<Type>` if the script does not extend from Node).

```cs
public partial class UIVignette : ColorRect
{
    public override void _Ready()
    {
        // Set 'persistent' to true if this script is an autoload
        // Scripts that do not extend from Node are persistent by default

        // Non-persistent services are removed just before a scene change
        // Example of a persistent service: AudioManager, which should exist
        // throughout the game's duration

        // This UIVignette is part of the scene, so it should not be persistent
        Global.Services.Add(this, persistent: false); 
    }

    public void LightPulse() { ... }
}
```

With this setup, you can now retrieve the instance of `UIVignette` from anywhere in your code without relying on static properties or lengthy `GetNode<T>` paths.

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

Most method parameters are supported, allowing for more dynamic interactions
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
The state manager employs functions as states instead of using classes for state management. The [`State`](https://github.com/ValksGodotTools/GodotUtils/blob/ccd37342ab8d758a664d2abd3375a21b608d2198/State.cs) class is provided in the GodotUtils submodule. Below an example is provided to illustrate this approach.

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

### Mod Loader
> [!NOTE]
> Mods have the ability to swap out game assets and run C# scripts. However, there's a significant issue with C# mod scripts, which is detailed at https://github.com/ValksGodotTools/Template/issues/15. You can find an example mod repository [here](https://github.com/ValksGodotTools/ExampleMod).

## Tips
> [!TIP]
> To run code just before the game exits, you can subscribe to the `OnQuit` event.
> ```cs
> // This is an async function because you way want to await certain processes before the game exists
> Global.Services.Get<Global>().OnQuit += async () =>
> {
>     // Execute your code here
>     await Task.FromResult(1);
> }
> ```

## FAQ
### Q: I'm encountering errors when I first load the project. What should I do?
A: If you're seeing errors on your first project load, it could be because you're offline or didn't clone the submodules. An internet connection is required when running the project for the first time, after that it's not required. To get submodules, use:

```sh
git submodule update --init --recursive
```

### Q: The left hand in all the FPS animations is sticking to where the right hand is. How can I fix this?
A: Simply closing and reopening the Godot editor should resolve the issue.

### Q: I'm encountering errors in the console after switching to the `X` genre. What could be the issue?
A: Here are a few common reasons and how to fix them:
  - Make sure you've closed and reopened the Godot editor after changing genres.
  - If you accidentally clicked "Save Changes" on a popup, you will most likely need to download the repository again and start fresh.
  - If you saved a scene that the setup script removed, simply delete that scene, and the console errors should clear up.

### Q: My issue isn't listed here. What should I do?
A: Feel free to search for your issue in the repository's [issues section](https://github.com/ValksGodotTools/Template/issues). If it hasn't been reported yet, please open a new issue, and I'll be happy to help you.
    
## Contributing
Before you jump into contributing, take a moment to review the [Coding Style Guidelines](https://github.com/Valks-Games/sankari/wiki/Code-Style). If you have any questions you can talk to me on Discord, my username is `valky5`.

## Credits
For all credit to in-game assets used, see [credits.txt](https://github.com/ValksGodotTools/Template/blob/main/credits.txt).  

Huge thank you to the people in the [Godot Café Discord](https://discord.com/invite/zH7NUgz) for answering all my questions.
