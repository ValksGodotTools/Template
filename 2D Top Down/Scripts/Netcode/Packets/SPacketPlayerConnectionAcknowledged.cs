namespace Template;

using GodotUtils.Netcode;
using GodotUtils.Netcode.Client;

/// <summary>
/// The server has acknowledged this players connection. The server is informing
/// the client of this.
/// </summary>
public class SPacketPlayerConnectionAcknowledged : ServerPacket
{
    public Dictionary<uint, PlayerData> OtherPlayers { get; set; }

    public override void Write(PacketWriter writer)
    {
        writer.Write((int)OtherPlayers.Count);

        foreach (KeyValuePair<uint, PlayerData> pair in OtherPlayers)
        {
            writer.Write((uint)pair.Key);
            writer.Write((Vector2)pair.Value.Position);
        }
    }

    public override void Read(PacketReader reader)
    {
        int playerCount = reader.ReadInt();

        OtherPlayers = new();

        for (int i = 0; i < playerCount; i++)
        {
            uint id = reader.ReadUInt();
            Vector2 position = reader.ReadVector2();

            OtherPlayers.Add(id, new()
            {
                Position = position
            });
        }
    }

    public override void Handle(ENetClient client)
    {
        client.Log("Client received server acknowledgement");

        Level level = Global.Services.Get<Level>();

        level.AddLocalPlayer();

        foreach (KeyValuePair<uint, PlayerData> pair in OtherPlayers)
        {
            level.AddOtherPlayer(pair.Key, pair.Value.Position);
        }
    }
}
