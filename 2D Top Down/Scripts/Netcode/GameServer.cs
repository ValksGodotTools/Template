namespace Template;

using ENet;
using GodotUtils.Netcode;
using GodotUtils.Netcode.Server;

public partial class GameServer : ENetServer
{
    public Dictionary<uint, PlayerData> Players { get; set; } = new();

    public Dictionary<uint, PlayerData> GetOtherPlayers(uint excludeId) => 
        Players
            .Where(x => x.Key != excludeId)
            .ToDictionary(x => x.Key, x => x.Value);

    protected override void Starting()
    {
        int heartbeat = 100;
        EmitLoop.SetDelay(heartbeat);
    }

    protected override void Emit()
    {
        if (Players.Count < 2)
            return;

        // Send all the other players positions to each player
        foreach (KeyValuePair<uint, PlayerData> pair in Players)
        {
            Send(new SPacketPlayerPositions
            {
                Positions = GetOtherPlayers(pair.Key)
                    .ToDictionary(x => x.Key, x => x.Value.Position)
            }, Peers[pair.Key]);
        }
    }

    protected override void Disconnected(Event netEvent)
    {
        Players.Remove(netEvent.Peer.ID);
    }

    protected override void Stopped()
    {
        
    }
}

public class PlayerData
{
    public Vector2 Position { get; set; }
}
