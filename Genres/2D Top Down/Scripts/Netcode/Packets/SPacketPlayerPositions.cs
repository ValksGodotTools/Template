namespace Template;

using Template.Netcode;
using Template.Netcode.Client;

public class SPacketPlayerPositions : ServerPacket
{
    public Dictionary<uint, Vector2> Positions { get; set; }

    public override void Write(PacketWriter writer)
    {
        writer.Write((byte)Positions.Count);

        foreach (KeyValuePair<uint, Vector2> pair in Positions)
        {
            writer.Write((uint)pair.Key);
            writer.Write((Vector2)pair.Value);
        }
    }

    public override void Read(PacketReader reader)
    {
        Positions = new();

        byte count = reader.ReadByte();

        for (int i = 0; i < count; i++)
        {
            uint id = reader.ReadUInt();
            Vector2 position = reader.ReadVector2();

            Positions.Add(id, position);
        }
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
