namespace Template;

using Template.TopDown2D;

public partial class Level : Node
{
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
            };
        };
    }

    public void AddLocalPlayer()
    {
        Player = GU.LoadPrefab<Player>("player");
        AddChild(Player);
        Player.Position = Net.PlayerSpawnPosition;
    }

    public void AddOtherPlayer(uint id, PlayerData playerData)
    {
        OtherPlayer otherPlayer = GU.LoadPrefab<OtherPlayer>("other_player");

        otherPlayer.LastServerPosition = playerData.Position;
        AddChild(otherPlayer);
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
