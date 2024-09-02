namespace Template;

using Template.TopDown2D;

public partial class Level : Node, INetLevel
{
    [Export] Node entities;
    [Export] PlayerCamera playerCamera;
    [Export] RoomTransitions roomTransitions;

    public Player Player { get; set; }
    public Dictionary<uint, OtherPlayer> OtherPlayers { get; set; } = new();

    public override void _Ready()
    {
        Global.Services.Add(this);
        
        Net net = Global.Services.Get<Net>();

        net.OnClientCreated += client =>
        {
            client.OnDisconnected += opcode =>
            {
                // The entire scene cannot be reset here because this will also reset the
                // instance stored for both GameServer and GameClient. These run on separate
                // threads, so resetting them here won't stop them on the other threads. Not
                // to mention they shouldn't be reset in the first place! So this is why the
                // entire scene is no longer reset when the client disconnects.
                // See https://github.com/ValksGodotTools/Template/issues/20 for more info
                // about this.
                Player.QueueFree();
                Player = null;

                OtherPlayers.Values.ForEach(x => x.QueueFree());
                OtherPlayers.Clear();

                playerCamera.StopFollowingPlayer();
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
