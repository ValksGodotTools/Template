namespace Template;

using Template.TopDown2D;

public partial class Level : Node
{
    public Player Player { get; set; }
    public Dictionary<uint, OtherPlayer> OtherPlayers { get; set; } = new();

    public override void _Ready()
    {
        Global.Services.Add(this);
        
        Net net = Global.Services.Get<Net>();

        net.OnClientCreated += client =>
        {
            client.OnDisconnected += opcode =>
            {
                Game.SceneManager.ResetCurrentScene();
            };
        };
    }

    public void AddLocalPlayer()
    {
        Player = GU.LoadPrefab<Player>("player");
        AddChild(Player);
        Player.Position = new Vector2(100, 100);
        Player.StartNet();
    }

    public void AddOtherPlayer(uint id, Vector2 position)
    {
        OtherPlayer otherPlayer = GU.LoadPrefab<OtherPlayer>("other_player");

        AddChild(otherPlayer);
        otherPlayer.Position = position;
        otherPlayer.SetLabelText(id + "");

        OtherPlayers.Add(id, otherPlayer);
    }

    public void RemoveOtherPlayer(uint id)
    {
        OtherPlayers[id].QueueFree();
        OtherPlayers.Remove(id);
    }
}
