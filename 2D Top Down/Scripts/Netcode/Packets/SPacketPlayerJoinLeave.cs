namespace Template;

using GodotUtils.Netcode;
using GodotUtils.Netcode.Client;

/// <summary>
/// A other player has joined or left the server.
/// </summary>
public class SPacketPlayerJoinLeave : ServerPacket
{
    public uint Id { get; set; }
    public Vector2 Position { get; set; }
    public bool Joined { get; set; }

    public override void Write(PacketWriter writer)
    {
        writer.Write((uint)Id);
        writer.Write((bool)Joined);

        if (Joined)
            writer.Write((Vector2)Position);
    }

    public override void Read(PacketReader reader)
    {
        Id = reader.ReadUInt();
        Joined = reader.ReadBool();

        if (Joined)
            Position = reader.ReadVector2();
    }

    public override void Handle(ENetClient client)
    {
        Level level = Global.Services.Get<Level>();

        if (Joined)
        {
            level.AddOtherPlayer(Id, Position);
        }
        else
        {
            level.RemoveOtherPlayer(Id);
        }
    }
}
