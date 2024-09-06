using ENet;
using System.Collections.Generic;
using System;
using Template.Netcode.Client;

namespace Template.Netcode;


public abstract class ServerPacket : GamePacket
{
    public static Dictionary<Type, PacketInfo<ServerPacket>> PacketMap { get; } = NetcodeUtils.MapPackets<ServerPacket>();
    public static Dictionary<byte, Type> PacketMapBytes { get; set; } = new();

    SendType sendType;

    public static void MapOpcodes()
    {
        foreach (KeyValuePair<Type, PacketInfo<ServerPacket>> packet in PacketMap)
            PacketMapBytes.Add(packet.Value.Opcode, packet.Key);
    }

    public void Send()
    {
        ENet.Packet enetPacket = CreateENetPacket();
        Peers[0].Send(ChannelId, ref enetPacket);
    }

    public void Broadcast(Host host)
    {
        ENet.Packet enetPacket = CreateENetPacket();

        if (Peers.Length == 0)
        {
            host.Broadcast(ChannelId, ref enetPacket);
        }
        else if (Peers.Length == 1)
        {
            host.Broadcast(ChannelId, ref enetPacket, Peers[0]);
        }
        else
        {
            host.Broadcast(ChannelId, ref enetPacket, Peers);
        }
    }

    public void SetSendType(SendType sendType) => this.sendType = sendType;
    public SendType GetSendType() => sendType;

    public override byte GetOpcode() => PacketMap[GetType()].Opcode;

    /// <summary>
    /// The packet handled client-side (Godot thread)
    /// </summary>
    public abstract void Handle(ENetClient client);
}

public enum SendType
{
    Peer,
    Broadcast
}

