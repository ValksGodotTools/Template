namespace Template.Netcode;

using ENet;

public abstract class GamePacket
{
    public static int MaxSize { get; } = 8192;

    protected Peer[] Peers { get; set; }
    protected byte ChannelId { get; }

    // Packets are reliable by default
    readonly PacketFlags packetFlags = PacketFlags.Reliable;
    long size;
    byte[] data;

    public void Write()
    {
        using (PacketWriter writer = new())
        {
            writer.Write(GetOpcode());
            this?.Write(writer);

            data = writer.Stream.ToArray();
            size = writer.Stream.Length;
        }
    }

    public void SetPeer(Peer peer) => Peers = new Peer[] { peer };
    public void SetPeers(Peer[] peers) => Peers = peers;
    public long GetSize() => size;
    public abstract byte GetOpcode();
    public abstract void Write(PacketWriter writer);
    public abstract void Read(PacketReader reader);

    protected ENet.Packet CreateENetPacket()
    {
        ENet.Packet enetPacket = default(ENet.Packet);
        enetPacket.Create(data, packetFlags);
        return enetPacket;
    }
}
