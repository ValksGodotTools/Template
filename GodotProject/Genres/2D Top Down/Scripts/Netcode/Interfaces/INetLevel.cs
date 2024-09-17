using System.Collections.Generic;

namespace Template.TopDown2D;

internal interface INetLevel
{
    public Player Player { get; set; }
    public Dictionary<uint, OtherPlayer> OtherPlayers { get; set; }

    void AddLocalPlayer();
    void AddOtherPlayer(uint id, PlayerData playerData);
    void RemoveOtherPlayer(uint id);
}

