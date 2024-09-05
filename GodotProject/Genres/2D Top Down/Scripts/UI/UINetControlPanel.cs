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

public partial class UINetControlPanel : Control
{
    Net net;
    string ip = "127.0.0.1";
    ushort port = 25565;
    string username = "";
    string prevUsername;

    public override void _Ready()
    {
        net = new();

        Button btnStartServer = GetNode<Button>("%Start Server");
        Button btnStopServer = GetNode<Button>("%Stop Server");

        btnStartServer.Pressed += net.StartServer;
        btnStopServer.Pressed += net.Server.Stop;

        GetNode<Button>("%Start Client").Pressed += () => net.StartClient(ip, port, username);
        GetNode<Button>("%Stop Client").Pressed += net.StopClient;

        GetNode<LineEdit>("%IP").TextChanged += text =>
        {
            string[] words = text.Split(":");

            ip = words[0];

            if (words.Length < 2)
                return;

            if (ushort.TryParse(words[1], out ushort result))
            {
                if (result.CountDigits() > 2)
                    port = result;
            }
        };

        LineEdit lineEditUsername = GetNode<LineEdit>("%Username");
        
        lineEditUsername.TextChanged += text =>
        {
            username = lineEditUsername.Filter(text => text.IsAlphaNumeric());
        };

        net.OnClientCreated += client =>
        {
            net.Client.OnConnected += () =>
            {
                if (!net.Server.IsRunning)
                {
                    // Server is not running and client connected to another server
                    // Client should not be able to start a server while connected to another server
                    btnStartServer.Disabled = true;
                    btnStopServer.Disabled = true;
                }

                GetTree().UnfocusCurrentControl();
            };

            net.Client.OnDisconnected += opcode =>
            {
                btnStartServer.Disabled = false;
                btnStopServer.Disabled = false;
            };
        };
    }

    public override void _PhysicsProcess(double delta)
    {
        net.Client?.HandlePackets();
    }
}

