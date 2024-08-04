namespace Template;

public partial class UINetControlPanel : Node
{
    Net net;

    public override void _Ready()
    {
        net = new();

        GetNode<Button>("%Start Server").Pressed += net.StartServer;
        GetNode<Button>("%Stop Server").Pressed += () => net.Server.Stop();

        GetNode<Button>("%Start Client").Pressed += net.StartClient;
        GetNode<Button>("%Stop Client").Pressed += net.StopClient;
    }

    public override void _PhysicsProcess(double delta)
    {
        net.Client?.HandlePackets();
    }
}
