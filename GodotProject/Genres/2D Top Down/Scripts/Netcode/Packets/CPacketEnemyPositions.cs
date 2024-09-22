using ENet;
using Godot;
using GodotUtils;
using System.Collections.Generic;
using Template.Netcode;
using Template.Netcode.Server;

namespace Template.TopDown2D;

public class CPacketEnemyPositions : ClientPacket
{
    [NetSend(1)]
    public Dictionary<ulong, Vector2> Positions { get; set; }

    public override void Handle(ENetServer server, Peer client)
    {
        Positions.PrintFormatted();
    }
}
