using Template.Netcode;

namespace Template.TopDown2D;

public partial class UINetControlPanel : UINetControlPanelLow
{
    public override void StartClientButtonPressed(string username)
    {
        Global.Services.Get<Level>().PlayerUsername = username;
    }

    public override IGameServerFactory GameServerFactory()
    {
        return new GameServerFactory();
    }

    public override IGameClientFactory GameClientFactory()
    {
        return new GameClientFactory();
    }
}
