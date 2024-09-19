using Godot;
using GodotUtils;
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

