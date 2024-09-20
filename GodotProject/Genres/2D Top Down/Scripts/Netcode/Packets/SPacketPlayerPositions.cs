using Godot;
using System.Collections.Generic;
using Template.Netcode;
using Template.Netcode.Client;

namespace Template.TopDown2D;

public class SPacketPlayerPositions : ServerPacket
{
    [NetSend(1)]
    public Dictionary<uint, Vector2> Positions { get; set; }

    public override void Handle(ENetClient client)
    {
        Level level = ServiceProvider.Services.Get<Level>();

        foreach (KeyValuePair <uint, Vector2> pair in Positions)
        {
            if (level.OtherPlayers.TryGetValue(pair.Key, out OtherPlayer otherPlayer))
            {
                otherPlayer.LastServerPosition = pair.Value;
            }
        }

        // Send a client position packet to the server immediately right after
        // a server positions packet is received
        level.Player.NetSendPosition();
    }
}

