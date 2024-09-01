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
        writer.Write(OtherPlayers);
    }

    public override void Read(PacketReader reader)
    {
        OtherPlayers = reader.Read<Dictionary<uint, PlayerData>>();
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
