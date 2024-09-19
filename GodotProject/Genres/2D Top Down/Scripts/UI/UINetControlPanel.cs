using CSharpUtils;
using Godot;
using GodotUtils;

namespace Template.TopDown2D;

public partial class UINetControlPanel : Control
{
    private Net _net;
    private string _ip = "127.0.0.1";
    private ushort _port = 25565;
    private string _username = "";
    private string _prevUsername;

    public override void _Ready()
    {
        _net = GetTree().GetAutoload<Net>("Net");
        _net.Initialize(new GameServerFactory(), new GameClientFactory());

        GetNode<Button>("%Start Server").Pressed += _net.StartServer;
        GetNode<Button>("%Stop Server").Pressed += _net.StopServer;

        GetNode<Button>("%Start Client").Pressed += () => _net.StartClient(_ip, _port, _username);
        GetNode<Button>("%Stop Client").Pressed += _net.StopClient;

        GetNode<LineEdit>("%IP").TextChanged += text =>
        {
            string[] words = text.Split(":");

            _ip = words[0];

            if (words.Length < 2)
                return;

            if (ushort.TryParse(words[1], out ushort result))
            {
                if (result.CountDigits() > 2)
                    _port = result;
            }
        };

        LineEdit lineEditUsername = GetNode<LineEdit>("%Username");
        
        lineEditUsername.TextChanged += text =>
        {
            _username = lineEditUsername.Filter(text => text.IsAlphaNumeric());
        };

        // Future note to self: Working with this event has been proven extremely difficult
        // Try to avoid using if at all possible or figure out why it's so annoying to work with
        // Maybe try to do whatever you need to do outside the event somehow. Perhaps directly inside
        // Net.cs
        _net.OnClientCreated += client =>
        {
            _net.Client.OnConnected += () =>
            {
                if (!IsInstanceValid(this))
                {
                    return;
                }

                if (!_net.Server.IsRunning)
                {
                    // Server is not running and client connected to another server
                    // Client should not be able to start a server while connected to another server
                    GetNode<Button>("%Start Server").Disabled = true;
                    GetNode<Button>("%Stop Server").Disabled = true;
                }
            };

            _net.Client.OnDisconnected += opcode =>
            {
                if (!IsInstanceValid(this))
                {
                    return;
                }

                GetNode<Button>("%Start Server").Disabled = false;
                GetNode<Button>("%Stop Server").Disabled = false;
            };
        };
    }

    public override void _PhysicsProcess(double delta)
    {
        _net.Client?.HandlePackets();
    }
}

