using CSharpUtils;
using Godot;
using GodotUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Template;

using Template.Netcode;
using Template.Netcode.Client;

public class SPacketPlayerPositions : ServerPacket
{
    [NetSend(1)]
    public Dictionary<uint, Vector2> Positions { get; set; }

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

