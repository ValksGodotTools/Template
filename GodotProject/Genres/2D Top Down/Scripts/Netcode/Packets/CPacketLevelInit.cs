using ENet;
using Godot;
using System.Collections.Generic;
using System.Linq;
using Template.Netcode;
using Template.Netcode.Server;

namespace Template.TopDown2D;

public class CPacketLevelInit : ClientPacket
{
    [NetSend(1)]
    public List<ulong> EnemyInstanceIds { get; set; }

    public override void Handle(ENetServer server, Peer client)
    {
        GameServer gameServer = (GameServer)server;
        gameServer.Enemies = EnemyInstanceIds.ToDictionary(x => x, x => Vector2.Zero);
    }
}
