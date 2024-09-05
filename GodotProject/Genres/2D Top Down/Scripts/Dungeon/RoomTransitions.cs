using Godot;
using GodotUtils;
using System.Collections.Generic;

using Template.TopDown2D;

namespace Template;

public partial class RoomTransitions : Node
{
    [Export] TileMapLayer tileMap; // Reference to the tile map layer
    [Export] Camera2D playerCamera; // Reference to the player's camera

    Player player; // Reference to the player object

    Vector2I currentRoom; // Current room coordinates
    Vector2I roomSize; // Size of each room
    Vector2I tileSize; // Size of each tile

    // List to keep track of room boundary nodes
    readonly List<Node2D> roomBoundNodes = new();

    public override void _Ready()
    {
        InitializeRoomSize();
    }

    // Initializes the size of each room based on tile size and scale
    private void InitializeRoomSize()
    {
        tileSize = tileMap.TileSet.TileSize;
        int roomTileSize = 10;

        int roomWidth = (int)tileMap.Scale.X * tileSize.X * roomTileSize;
        int roomHeight = (int)tileMap.Scale.Y * tileSize.Y * roomTileSize;

        roomSize = new(roomWidth, roomHeight);
    }

    // Initializes the room transitions with the player object
    public void Init(Player player)
    {
        this.player = player;
        LimitCameraBoundsToRoom();
        CreateRoomBoundaries();
        CreateRoomDoorTriggers();
    }

    // Resets the room transitions to initial state
    public void Reset()
    {
        currentRoom = Vector2I.Zero;
        ClearRoomBoundNodes();
    }

    // Clears all room boundary nodes
    private void ClearRoomBoundNodes()
    {
        roomBoundNodes.ForEach(node => node.QueueFree());
        roomBoundNodes.Clear();
    }

    // Removes camera bounds to allow free movement
    private void UnlimitedCameraBounds()
    {
        playerCamera.LimitTop = int.MinValue;
        playerCamera.LimitLeft = int.MinValue;
        playerCamera.LimitBottom = int.MaxValue;
        playerCamera.LimitRight = int.MaxValue;
    }

    // Creates a trigger for room transitions
    private void CreateRoomDoorTrigger(Vector2 position, Vector2I normal)
    {
        Area2D area = CreateArea2D("Room Door Trigger " + normal, false);
        CollisionShape2D collision = CreateWorldBoundaryShape(normal, Colors.Green);

        area.BodyEntered += body =>
        {
            if (body is Player)
                HandleRoomTransition(normal);
        };

        area.AddChild(collision);
        AddChild(area);
        area.Position = position;

        roomBoundNodes.Add(area);
    }

    // Creates an Area2D node
    private Area2D CreateArea2D(string name, bool monitorable)
    {
        return new Area2D
        {
            Name = name,
            Monitorable = monitorable
        };
    }

    // Creates a CollisionShape2D with a WorldBoundaryShape2D
    private CollisionShape2D CreateWorldBoundaryShape(Vector2I normal, Color color)
    {
        return new CollisionShape2D
        {
            Shape = new WorldBoundaryShape2D
            {
                Normal = normal
            },
            Modulate = color
        };
    }

    // Handles the room transition when a trigger is entered
    private void HandleRoomTransition(Vector2I normal)
    {
        GD.Print("Entered the trigger " + normal);

        if (IsValidTransitionNormal(normal))
        {
            TransitionToNextRoom(normal);
        }
    }

    // Checks if the transition normal is valid
    private bool IsValidTransitionNormal(Vector2I normal)
    {
        return normal == Vector2I.Right ||
               normal == Vector2I.Left ||
               normal == Vector2I.Up ||
               normal == Vector2I.Down;
    }

    // Transitions to the next room based on the normal vector
    private void TransitionToNextRoom(Vector2I normal)
    {
        currentRoom += normal * -1;

        ClearRoomBoundNodes();
        PrepareCameraForTransition();
        AnimateCameraAndPlayer(normal);
    }

    // Prepares the camera for the transition animation
    private void PrepareCameraForTransition()
    {
        playerCamera.SetPhysicsProcess(false);
        playerCamera.PositionSmoothingEnabled = false;
        playerCamera.Position = playerCamera.GetScreenCenterPosition();
        UnlimitedCameraBounds();
    }

    // Animates the camera and player during the transition
    private void AnimateCameraAndPlayer(Vector2I normal)
    {
        double duration = 1;
        Vector2 screenSize = GetViewport().GetVisibleRect().Size;
        Vector2 transitionOffset = screenSize * (normal * -1);

        new GTween(playerCamera)
            .SetAnimatingProp(Camera2D.PropertyName.Position)
            .AnimateProp(playerCamera.Position + transitionOffset, duration)
            .TransExpo();

        new GTween(player)
            .SetAnimatingProp(Player.PropertyName.Position)
            .AnimateProp(player.Position + new Vector2(150, 150) * (normal * -1), duration)
            .EaseIn()
            .Callback(FinalizeRoomTransition);
    }

    // Finalizes the room transition after the animation
    private void FinalizeRoomTransition()
    {
        playerCamera.SetPhysicsProcess(true);
        playerCamera.PositionSmoothingEnabled = true;
        LimitCameraBoundsToRoom();
        CreateRoomBoundaries();
        CreateRoomDoorTriggers();
    }

    // Limits the camera bounds to the current room
    private void LimitCameraBoundsToRoom()
    {
        playerCamera.LimitTop = roomSize.Y * currentRoom.Y;
        playerCamera.LimitLeft = roomSize.X * currentRoom.X;
        playerCamera.LimitBottom = roomSize.Y + (roomSize.Y * currentRoom.Y);
        playerCamera.LimitRight = roomSize.X + (roomSize.X * currentRoom.X);
    }

    // Creates triggers for room transitions at each door
    private void CreateRoomDoorTriggers()
    {
        Vector2 offset = new(32, 32);

        CreateRoomDoorTrigger(roomSize * currentRoom + offset, Vector2I.Down);
        CreateRoomDoorTrigger(roomSize * currentRoom + offset, Vector2I.Right);
        CreateRoomDoorTrigger(roomSize + (roomSize * currentRoom) - offset, Vector2I.Up);
        CreateRoomDoorTrigger(roomSize + (roomSize * currentRoom) - offset, Vector2I.Left);
    }

    // Creates boundaries for the current room
    private void CreateRoomBoundaries()
    {
        CreateWorldBoundary(roomSize * currentRoom, Vector2I.Down);
        CreateWorldBoundary(roomSize * currentRoom, Vector2I.Right);
        CreateWorldBoundary(roomSize + (roomSize * currentRoom), Vector2I.Up);
        CreateWorldBoundary(roomSize + (roomSize * currentRoom), Vector2I.Left);
    }

    // Creates a world boundary at the specified position and normal
    private void CreateWorldBoundary(Vector2 position, Vector2I normal)
    {
        StaticBody2D body = new()
        {
            Name = "Room Boundary " + normal
        };

        CollisionShape2D collision = CreateWorldBoundaryShape(normal, Colors.White);

        body.AddChild(collision);
        AddChild(body);
        body.Position = position;

        roomBoundNodes.Add(body);
    }
}

