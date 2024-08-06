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
        EmitLoop.SetDelay(Net.HeartbeatPosition);
    }

    protected override void Emit()
    {
        // If there are less than 2 players on the server there is no point in sending
        // position packets to the only player on the server
        if (Players.Count < 2)
            return;

        // Send all the other players positions to each player
        foreach (uint id in Players.Keys)
        {
            // Retrieve all players except for player with 'id'
            Dictionary<uint, PlayerData> otherPlayers = GetOtherPlayers(id)
                .Where(x => x.Value.Position != x.Value.PrevPosition)
                .ToDictionary(x => x.Key, x => x.Value);

            // Send these player positions to player with 'id'
            Send(new SPacketPlayerPositions
            {
                Positions = otherPlayers.ToDictionary(x => x.Key, x => x.Value.Position)
            }, Peers[id]);
        }

        // This must not be in the previous foreach loop above or weird things will happen
        // This has to execute AFTER the previous foreach loop has completed
        foreach (PlayerData player in Players.Values)
            player.PrevPosition = player.Position;
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
    public Vector2 PrevPosition { get; set; }
}
