using Template.Netcode;
using Template.Netcode.Client;
using Template.Netcode.Server;

namespace Template.TopDown2D;

public partial class UINetControlPanel : UINetControlPanelLow
{
    public override void StartClientButtonPressed(string username)
    {
        ServiceProvider.Services.Get<Level>().PlayerUsername = username;
    }

    public override IGameServerFactory GameServerFactory() => new GameServerFactory();
    public override IGameClientFactory GameClientFactory() => new GameClientFactory();
}

public class GameServerFactory : IGameServerFactory
{
    public ENetServer CreateServer() => new GameServer();
}

public class GameClientFactory : IGameClientFactory
{
    public ENetClient CreateClient() => new GameClient();
}
