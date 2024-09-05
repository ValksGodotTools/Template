namespace Template;

using Template.TopDown2D;

public partial class Level : Node, INetLevel
{
    [Export] Node entities;
    [Export] PlayerCamera playerCamera;
    [Export] RoomTransitions roomTransitions;

    public Player Player { get; set; }
    public Dictionary<uint, OtherPlayer> OtherPlayers { get; set; } = new();

    public override void _Ready()
    {
        Global.Services.Add(this);

        Game.Net.OnClientCreated += client =>
        {
            client.OnDisconnected += opcode =>
            {
                // The entire scene cannot be reset here because this will also reset the
                // instance stored for both GameServer and GameClient. These run on separate
                // threads, so resetting them here won't stop them on the other threads. Not
                // to mention they shouldn't be reset in the first place! So this is why the
                // entire scene is no longer reset when the client disconnects.
                // See https://github.com/ValksGodotTools/Template/issues/20 for more info
                // about this.
                Player.QueueFree();
                Player = null;

                OtherPlayers.Values.ForEach(x => x.QueueFree());
                OtherPlayers.Clear();

                playerCamera.StopFollowingPlayer();
                roomTransitions.Reset();
            };
        };

        GetRootControlNodes(this);

        // This should not initially be set to DisplayServer.WindowGetSize() because then UI sizes
        // will be inconsistent if the player starts the game in fullscreen vs in windowed mode
        Vector2 referenceWindowSize = new(1280, 720);
        float scaleOffset = 0;

        SetRootControlPositions(referenceWindowSize, scaleOffset);

        GetTree().Root.GetViewport().SizeChanged += () =>
        {
            SetRootControlPositions(referenceWindowSize, scaleOffset);
        };
    }

    private static void SetRootControlPositions(Vector2 initialWindowSize, float scaleOffset)
    {
        foreach (Control infoPanel in rootControls)
        {
            float scaleFactor = initialWindowSize.X / DisplayServer.WindowGetSize().X;
            Vector2 newScale = Vector2.One * (scaleFactor + scaleOffset);

            // Calculate the new position and size based on the original position and size
            Vector2 originalPosition = infoPanel.GetRect().Position;
            Vector2 originalSize = infoPanel.GetRect().Size;

            Vector2 newPosition = originalPosition * newScale;
            Vector2 newSize = originalSize * newScale;

            Rect2 rect = infoPanel.GetRect();

            // Apply the new position and size
            rect.Position = newPosition;
            rect.Size = newSize;
        }
    }

    private static List<Control> rootControls = [];

    private void GetRootControlNodes(Node node)
    {
        if (node is Control controlNode)
        {
            rootControls.Add(controlNode);
            return;
        }

        foreach (Node child in node.GetChildren())
        {
            GetRootControlNodes(child);
        }
    }

    public void AddLocalPlayer()
    {
        Player = Game.LoadPrefab<Player>(Prefab.PlayerMain);
        Player.Position = Net.PlayerSpawnPosition;
        entities.AddChild(Player);

        playerCamera.StartFollowingPlayer(Player);
        roomTransitions.Init(Player);
    }

    public void AddOtherPlayer(uint id, PlayerData playerData)
    {
        OtherPlayer otherPlayer = Game.LoadPrefab<OtherPlayer>(Prefab.PlayerOther);

        otherPlayer.LastServerPosition = playerData.Position;
        entities.AddChild(otherPlayer);
        otherPlayer.Position = playerData.Position;
        otherPlayer.SetLabelText($"{playerData.Username} ({id})");

        OtherPlayers.Add(id, otherPlayer);
    }

    public void RemoveOtherPlayer(uint id)
    {
        OtherPlayers[id].QueueFree();
        OtherPlayers.Remove(id);
    }
}
