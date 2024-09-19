using CSharpUtils;
using Godot;
using System.Collections.Generic;

namespace Template.TopDown2D;

public partial class Level : Node, INetLevel
{
    [Export] private Node entities;
    [Export] private PlayerCamera playerCamera;
    [Export] private RoomTransitions roomTransitions;

    public Player Player { get; set; }
    public Dictionary<uint, OtherPlayer> OtherPlayers { get; set; } = [];

    public override void _Ready()
    {
        Global.Services.Add(this);

        Global.Services.Get<Net>().OnClientCreated += client =>
        {
            client.OnDisconnected += opcode =>
            {
                // WARNING:
                // Do not reset world here with Global.Servers.Get<SceneManager>().ResetCurrentScene()
                //
                // REASON:
                // Resetting the world will reset the contents of Net.cs and Level.cs which will result
                // in numerous problems. Attempts have been made to turn Net into a persistent autoload
                // however doing so will require Player and OtherPlayers to be defined in Net.cs and
                // doing stuff like playerCamera.StartFollowingPlayer(Player); and entities.AddChild(otherPlayer);
                // will need to be done in Net.cs because Level.cs will get reset. Getting the playerCamera
                // and a path to the entities node from Net.cs is miserable. Imagine doing
                // GetTree().Root.GetNode<PlayerCamera>("/root/Level/Camera2D")...
                // Another reason to avoid resetting the entire world is to avoid seeing the lag created
                // from the world reset.

                Player.QueueFree();
                Player = null;

                OtherPlayers.Values.ForEach(x => x.QueueFree());
                OtherPlayers.Clear();

                playerCamera.StopFollowingPlayer();
                roomTransitions.Reset();
            };
        };
    }

    public void AddLocalPlayer()
    {
        Player = Game.LoadPrefab<Player>(Prefab.PlayerMain);
        Player.Position = Net.PlayerSpawnPosition;
        entities.AddChild(Player);

        playerCamera.StartFollowingPlayer(Player);
        roomTransitions.Init(Player);
    }

    public void AddOtherPlayer(uint id, PlayerData playerData)
    {
        OtherPlayer otherPlayer = Game.LoadPrefab<OtherPlayer>(Prefab.PlayerOther);

        otherPlayer.LastServerPosition = playerData.Position;
        entities.AddChild(otherPlayer);
        otherPlayer.Position = playerData.Position;
        otherPlayer.SetLabelText($"{playerData.Username} ({id})");

        OtherPlayers.Add(id, otherPlayer);
    }

    public void RemoveOtherPlayer(uint id)
    {
        OtherPlayers[id].QueueFree();
        OtherPlayers.Remove(id);
    }
}

