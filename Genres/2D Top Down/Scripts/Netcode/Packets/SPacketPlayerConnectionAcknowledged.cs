namespace Template;

using Template.Netcode;
using Template.Netcode.Client;

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
            writer.Write((string)pair.Value.Username);
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
            string username = reader.ReadString();
            Vector2 position = reader.ReadVector2();

            OtherPlayers.Add(id, new()
            {
                Username = username,
                Position = position
            });
        }
    }

    public override void Handle(ENetClient client)
    {
        client.Log("Client received server acknowledgement");

        INetLevel level = Global.Services.Get<Level>();

        level.AddLocalPlayer();

        foreach (KeyValuePair<uint, PlayerData> pair in OtherPlayers)
        {
            level.AddOtherPlayer(pair.Key, pair.Value);
        }
    }
}
