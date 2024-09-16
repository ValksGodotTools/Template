using CSharpUtils;
using Godot;
using GodotUtils;

namespace Template;

public partial class UINetControlPanel : Control
{
    private Net _net;
    private string _ip = "127.0.0.1";
    private ushort _port = 25565;
    private string _username = "";
    private string _prevUsername;

    public override void _Ready()
    {
        _net = new();

        Button btnStartServer = GetNode<Button>("%Start Server");
        Button btnStopServer = GetNode<Button>("%Stop Server");

        btnStartServer.Pressed += _net.StartServer;
        btnStopServer.Pressed += _net.Server.Stop;

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

