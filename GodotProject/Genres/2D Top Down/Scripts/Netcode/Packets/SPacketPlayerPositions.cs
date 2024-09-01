﻿namespace Template;

using Template.Netcode;
using Template.Netcode.Client;

public class SPacketPlayerPositions : ServerPacket
{
    public Dictionary<uint, Vector2> Positions { get; set; }

    public override void Write(PacketWriter writer)
    {
        writer.Write(Positions);
    }

    public override void Read(PacketReader reader)
    {
        Positions = reader.Read<Dictionary<uint, Vector2>>();
    }

    public override void Handle(ENetClient client)
    {
        INetLevel level = Global.Services.Get<Level>();

        foreach (KeyValuePair <uint, Vector2> pair in Positions)
        {
            if (level.OtherPlayers.ContainsKey(pair.Key))
                level.OtherPlayers[pair.Key].LastServerPosition = pair.Value;
        }

        // Send a client position packet to the server immediately right after
        // a server positions packet is received
        level.Player.NetSendPosition();
    }
}
