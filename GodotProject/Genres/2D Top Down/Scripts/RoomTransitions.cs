using Template.TopDown2D;

namespace Template;

public partial class RoomTransitions : Node
{
    [Export] TileMapLayer tileMap;
    [Export] Camera2D playerCamera;

    Player player;

    Vector2I currentRoom;
    Vector2I roomSize;
    Vector2I tileSize;

    readonly List<Node2D> roomBoundNodes = new();

    public override void _Ready()
    {
        tileSize = tileMap.TileSet.TileSize;
        int room_tile_size = 10;

        int room_width = (int)tileMap.Scale.X * tileSize.X * room_tile_size;
        int room_height = (int)tileMap.Scale.Y * tileSize.Y * room_tile_size;

        roomSize = new(room_width, room_height);
    }

    public void Init(Player player)
    {
        this.player = player;
        LimitCameraBoundsToRoom();
        CreateRoomBoundaries();
        CreateRoomDoorTriggers();
    }

    void UnlimitedCameraBounds()
    {
        playerCamera.LimitTop = int.MinValue;
        playerCamera.LimitLeft = int.MinValue;
        playerCamera.LimitBottom = int.MaxValue;
        playerCamera.LimitRight = int.MaxValue;
    }

    void CreateRoomDoorTrigger(Vector2 position, Vector2I normal)
    {
        Area2D area = new()
        {
            Name = "Room Door Trigger " + normal,
            Monitorable = false
        };

        CollisionShape2D collision = new()
        {
            Shape = new WorldBoundaryShape2D
            {
                Normal = normal
            },
            Modulate = Colors.Green
        };

        area.BodyEntered += body =>
        {
            if (body is not Player)
                return;

            GD.Print("Entered the trigger " + normal);

            if (normal == new Vector2I(1, 0) ||
                normal == new Vector2I(-1, 0) ||
                normal == new Vector2I(0, -1) ||
                normal == new Vector2I(0, 1))
            {
                TransitionToNextRoom(normal);
            }
        };

        area.AddChild(collision);
        AddChild(area);
        area.Position = position;

        roomBoundNodes.Add(area);
    }

    void TransitionToNextRoom(Vector2I normal)
    {
        currentRoom += normal * -1;

        roomBoundNodes.ForEach(x => x.QueueFree());
        roomBoundNodes.Clear();

        playerCamera.SetPhysicsProcess(false);
        playerCamera.PositionSmoothingEnabled = false;
        playerCamera.Position = playerCamera.GetScreenCenterPosition();

        UnlimitedCameraBounds();

        double duration = 1;

        new GTween(playerCamera)
            .SetAnimatingProp(Camera2D.PropertyName.Position)
            .AnimateProp(playerCamera.Position + GetViewport().GetVisibleRect().Size * (normal * -1), duration)
                .TransExpo();

        new GTween(player)
            .SetAnimatingProp(Player.PropertyName.Position)
            .AnimateProp(player.Position + new Vector2(150, 150) * (normal * -1), duration)
                .EaseIn()
            .Callback(() =>
            {
                playerCamera.SetPhysicsProcess(true);
                playerCamera.PositionSmoothingEnabled = true;
                LimitCameraBoundsToRoom();
                CreateRoomBoundaries();
                CreateRoomDoorTriggers();
            });
    }

    void LimitCameraBoundsToRoom()
    {
        playerCamera.LimitTop = roomSize.Y * currentRoom.Y;
        playerCamera.LimitLeft = roomSize.X * currentRoom.X;
        playerCamera.LimitBottom = roomSize.Y + (roomSize.Y * currentRoom.Y);
        playerCamera.LimitRight = roomSize.X + (roomSize.X * currentRoom.X);
    }

    void CreateRoomDoorTriggers()
    {
        Vector2 offset = new(32, 32);

        CreateRoomDoorTrigger(roomSize * currentRoom + offset, new(0, 1));
        CreateRoomDoorTrigger(roomSize * currentRoom + offset, new(1, 0));
        CreateRoomDoorTrigger(roomSize + (roomSize * currentRoom) - offset, new(0, -1));
        CreateRoomDoorTrigger(roomSize + (roomSize * currentRoom) - offset, new(-1, 0));
    }

    void CreateRoomBoundaries()
    {
        CreateWorldBoundary(roomSize * currentRoom, new(0, 1));
        CreateWorldBoundary(roomSize * currentRoom, new(1, 0));
        CreateWorldBoundary(roomSize + (roomSize * currentRoom), new(0, -1));
        CreateWorldBoundary(roomSize + (roomSize * currentRoom), new(-1, 0));
    }

    void CreateWorldBoundary(Vector2 position, Vector2 normal)
    {
        StaticBody2D body = new()
        {
            Name = "Room Boundary " + normal
        };

        CollisionShape2D collision = new()
        {
            Shape = new WorldBoundaryShape2D
            {
                Normal = normal
            }
        };

        body.AddChild(collision);
        AddChild(body);
        body.Position = position;

        roomBoundNodes.Add(body);
    }
}
