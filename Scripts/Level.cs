namespace Template;

using System.IO;
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
                Player.QueueFree();
            };
        };
    }

    public void AddLocalPlayer()
    {
        Player = Prefabs.Player;
        AddChild(Player);
        Player.Position = new Vector2(100, 100);
        Player.StartNet();
    }

    public void AddOtherPlayer(uint id, Vector2 position)
    {
        OtherPlayer otherPlayer = Prefabs.OtherPlayer;

        AddChild(otherPlayer);
        otherPlayer.Position = position;
        otherPlayer.SetLabelText(id + "");

        OtherPlayers.Add(id, otherPlayer);
    }

    public void RemoveOtherPlayer(uint id)
    {
        OtherPlayers[id].QueueFree();
        OtherPlayers.Remove(id);
    }

    private class Prefabs
    {
        public static Player Player { get; } = GU.LoadPrefab<Player>("player");
        public static OtherPlayer OtherPlayer { get; } = GU.LoadPrefab<OtherPlayer>("other_player");
    }
}
