namespace Template;

public partial class UINetControlPanel : Node
{
    Net net;

    public override void _Ready()
    {
        net = new();

        Button btnStartServer = GetNode<Button>("%Start Server");
        Button btnStopServer = GetNode<Button>("%Stop Server");

        btnStartServer.Pressed += net.StartServer;
        btnStopServer.Pressed += () => net.Server.Stop();

        GetNode<Button>("%Start Client").Pressed += net.StartClient;
        GetNode<Button>("%Stop Client").Pressed += net.StopClient;

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
