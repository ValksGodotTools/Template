namespace Template;

using ENet;
using Template.Netcode;
using Template.Netcode.Server;

public class CPacketPosition : ClientPacket
{
    [NetSend(1)]
    public Vector2 Position { get; set; }

    public override void Handle(ENetServer s, Peer client)
    {
        GameServer server = (GameServer)s;
        server.Players[client.ID].Position = Position;
    }
}
