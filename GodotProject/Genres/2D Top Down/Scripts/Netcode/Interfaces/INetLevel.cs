using System.Collections.Generic;
using Template.TopDown2D;

namespace Template;

internal interface INetLevel
{
    public Player Player { get; set; }
    public Dictionary<uint, OtherPlayer> OtherPlayers { get; set; }

    public void AddLocalPlayer();
    public void AddOtherPlayer(uint id, PlayerData playerData);
    public void RemoveOtherPlayer(uint id);
}

