using Template.Netcode;
using Template.Netcode.Server;

namespace Template.TopDown2D;

public class GameServerFactory : IGameServerFactory
{
    public ENetServer CreateServer()
    {
        return new GameServer();
    }
}

