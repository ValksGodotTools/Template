namespace Template;

using ENet;
using Template.Netcode;
using Template.Netcode.Server;

public class CPacketPosition : ClientPacket
{
    public Vector2 Position { get; set; }

    public override void Write(PacketWriter writer)
    {
        writer.Write((Vector2)Position);
    }

    public override void Read(PacketReader reader)
    {
        Position = reader.ReadVector2();
    }

    public override void Handle(ENetServer s, Peer client)
    {
        GameServer server = (GameServer)s;
        server.Players[client.ID].Position = Position;
    }
}
