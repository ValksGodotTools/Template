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
        // If there are less than 2 players on the server there is no point in sending
        // position packets to the only player on the server
        if (Players.Count < 2)
            return;

        // Send all the other players positions to each player
        foreach (uint id in Players.Keys)
        {
            // Retrieve all players except for player with 'id'
            Dictionary<uint, PlayerData> otherPlayers = GetOtherPlayers(id);

            // These are the players positions that will get sent over the network
            Dictionary<uint, Vector2> otherPlayerPositionsFinal = new();

            // Do not send a players position if it has not changed
            foreach (KeyValuePair<uint, PlayerData> otherPlayer in otherPlayers)
            {
                // The positions are not the same, it's okay to send this position
                if (otherPlayer.Value.Position != otherPlayer.Value.PrevPosition)
                {
                    // Keep track of this new position
                    otherPlayerPositionsFinal.Add(otherPlayer.Key, otherPlayer.Value.Position);
                }

                // Keep track of the previous position
                Players[otherPlayer.Key].PrevPosition = otherPlayer.Value.Position;
            }

            // Send these player positions to player with 'id'
            Send(new SPacketPlayerPositions
            {
                Positions = otherPlayerPositionsFinal
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
    public Vector2 PrevPosition { get; set; }
}
