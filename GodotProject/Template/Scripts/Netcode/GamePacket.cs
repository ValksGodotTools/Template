using CSharpUtils;
using Godot;
using GodotUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Template.Netcode;

using ENet;
using System.Reflection;

public abstract class GamePacket
{
    private PropertyInfo[] _cachedProperties;

    public static int MaxSize { get; } = 8192;

    protected Peer[] Peers { get; set; }
    protected byte ChannelId { get; }

    // Packets are reliable by default
    readonly PacketFlags packetFlags = PacketFlags.Reliable;
    long size;
    byte[] data;

    public void Write()
    {
        using PacketWriter writer = new();
        writer.Write(GetOpcode());
        this?.Write(writer);

        data = writer.Stream.ToArray();
        size = writer.Stream.Length;
    }

    public void SetPeer(Peer peer) => Peers = new Peer[] { peer };
    public void SetPeers(Peer[] peers) => Peers = peers;
    public long GetSize() => size;
    public abstract byte GetOpcode();

    public virtual void Write(PacketWriter writer)
    {
        PropertyInfo[] properties = GetProperties();

        foreach (PropertyInfo property in properties)
        {
            writer.Write(property.GetValue(this));
        }
    }

    public virtual void Read(PacketReader reader)
    {
        PropertyInfo[] properties = GetProperties();

        foreach (PropertyInfo property in properties)
        {
            property.SetValue(this, reader.Read(property.PropertyType));
        }
    }

    private PropertyInfo[] GetProperties()
    {
        if (_cachedProperties == null)
        {
            _cachedProperties = GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.GetCustomAttributes(typeof(NetSendAttribute), true).Any())
                .OrderBy(p => ((NetSendAttribute)p.GetCustomAttributes(typeof(NetSendAttribute), true).First()).Order)
                .ToArray();
        }

        return _cachedProperties;
    }

    protected ENet.Packet CreateENetPacket()
    {
        ENet.Packet enetPacket = default;
        enetPacket.Create(data, packetFlags);
        return enetPacket;
    }
}

