namespace Template;

using ENet;
using GodotUtils.Netcode;
using GodotUtils.Netcode.Server;

public partial class GameServer : ENetServer
{
    public const int HeartbeatPosition = 100;

    public Dictionary<uint, PlayerData> Players { get; set; } = new();

    public Dictionary<uint, PlayerData> GetOtherPlayers(uint excludeId) => 
        Players
            .Where(x => x.Key != excludeId)
            .ToDictionary(x => x.Key, x => x.Value);

    protected override void Starting()
    {
        EmitLoop.SetDelay(HeartbeatPosition);
    }

    protected override void Emit()
    {
        if (Players.Count < 2)
            return;

        // Send all the other players positions to each player
        foreach (uint id in Players.Keys)
        {
            Send(new SPacketPlayerPositions
            {
                Positions = GetOtherPlayers(id)
                    .ToDictionary(x => x.Key, x => x.Value.Position)
            }, Peers[id]);
        }
    }

    protected override void Disconnected(Event netEvent)
    {
        Players.Remove(netEvent.Peer.ID);

        // Tell everyone that this player has left
        foreach (uint id in Players.Keys)
        {
            if (id == netEvent.Peer.ID)
                continue;

            Send(new SPacketPlayerJoinLeave
            {
                Id = netEvent.Peer.ID,
                Joined = false
            }, Peers[id]);
        }
    }

    protected override void Stopped()
    {
        
    }
}

public class PlayerData
{
    public string Username { get; set; }
    public Vector2 Position { get; set; }
}
