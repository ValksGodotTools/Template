using CSharpUtils;
using Godot;
using GodotUtils;
using Template.Netcode;

namespace Template;

public abstract partial class UINetControlPanelLow : Control
{
    private Net _net;
    private string _ip = "127.0.0.1";
    private ushort _port = 25565;
    private string _username = "";
    private string _prevUsername;

    public abstract IGameServerFactory GameServerFactory();
    public abstract IGameClientFactory GameClientFactory();
    public abstract void StartClientButtonPressed(string username);

    public override void _Ready()
    {
        _net = new();
        _net.Initialize(GameServerFactory(), GameClientFactory());

        Button btnStartServer = GetNode<Button>("%Start Server");
        Button btnStopServer = GetNode<Button>("%Stop Server");

        btnStartServer.Pressed += _net.StartServer;
        btnStopServer.Pressed += _net.StopServer;

        GetNode<Button>("%Start Client").Pressed += () =>
        {
            StartClientButtonPressed(_username);
            _net.StartClient(_ip, _port);
        };

        GetNode<Button>("%Stop Client").Pressed += _net.StopClient;

        GetNode<LineEdit>("%IP").TextChanged += text =>
        {
            string[] words = text.Split(":");

            _ip = words[0];

            if (words.Length < 2)
            {
                return;
            }

            if (ushort.TryParse(words[1], out ushort result))
            {
                if (result.CountDigits() > 2)
                {
                    _port = result;
                }
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
                if (!_net.Server.IsRunning)
                {
                    // Server is not running and client connected to another server
                    // Client should not be able to start a server while connected to another server
                    btnStartServer.Disabled = true;
                    btnStopServer.Disabled = true;
                }

                GetTree().UnfocusCurrentControl();
            };

            _net.Client.OnDisconnected += opcode =>
            {
                btnStartServer.Disabled = false;
                btnStopServer.Disabled = false;
            };
        };
    }

    public override void _PhysicsProcess(double delta)
    {
        _net.Client?.HandlePackets();
    }
}

