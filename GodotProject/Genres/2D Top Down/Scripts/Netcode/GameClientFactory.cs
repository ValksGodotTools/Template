using Template.Netcode;
using Template.Netcode.Client;

namespace Template.TopDown2D;

public class GameClientFactory : IGameClientFactory
{
    public ENetClient CreateClient()
    {
        return new GameClient();
    }
}
