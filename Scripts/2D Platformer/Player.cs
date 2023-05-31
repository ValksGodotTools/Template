namespace Template.Platformer2D;

public partial class Player : CharacterBody2D
{
    float maxSpeed = 500;
    float acceleration = 40;
    float friction = 20;
    float gravity = 20;
    float jumpForce = 100;
    float jumpLoss = 7.5f;

    float jumpLossBuildUp;
    bool holdingJumpKey;

    Dictionary<AreaType, (Area2D, AreaData)> areas = new();

    public override void _Ready()
    {
        MotionMode = MotionModeEnum.Grounded;

        InitAreas();
    }

    public override void _PhysicsProcess(double delta)
    {
        var horzDir = Input.GetAxis("move_left", "move_right");
        var vel = Velocity;

        // Horizontal movement
        vel.X += horzDir * acceleration;
        vel.X = Utils.ClampAndDampen(vel.X, friction, maxSpeed);

        //
        // //IsColliding(AreaType.Floor)
        if (Input.IsActionJustPressed("jump") && IsOnFloor())
        {
            holdingJumpKey = true;
            jumpLossBuildUp = 0;
            vel.Y -= jumpForce;
        }

        if (Input.IsActionPressed("jump") && holdingJumpKey)
        {
            jumpLossBuildUp += jumpLoss;
            vel.Y -= Mathf.Max(0, jumpForce - jumpLossBuildUp);
        }

        if (Input.IsActionJustReleased("jump"))
        {
            holdingJumpKey = false;
        }

        // Gravity
        vel.Y += gravity;

        Velocity = vel;
        MoveAndSlide();
    }

    bool IsColliding(AreaType type) => areas[type].Item2.Detected;

    void InitAreas()
    {
        foreach (AreaType type in Enum.GetValues(typeof(AreaType)))
        {
            var areaData = new AreaData();
            areas[type] = (SetupArea(type, areaData), areaData);
        }
    }

    Area2D SetupArea(AreaType type, AreaData areaData)
    {
        var area = CreateArea(type);

        if (area == null)
            return null;

        area.BodyEntered += body =>
        {
            if (body is TileMap)
            {
                areaData.Detected = true;
                areaData.BodyCount++;
            }
        };

        area.BodyExited += body =>
        {
            if (body is TileMap)
            {
                areaData.BodyCount--;
                if (areaData.BodyCount == 0)
                    areaData.Detected = false;
            }
        };

        return area;
    }

    Area2D CreateArea(AreaType type)
    {
        var pixelSize = GetNode<Sprite2D>("Sprite2D").GetPixelSize();

        if (type == AreaType.Floor)
        {
            var floorHeight = 10;

            var area = new Area2D();
            var collisionShape = new CollisionShape2D
            {
                DebugColor = Colors.Pink,
                Position = new Vector2(0, pixelSize.Y / 2 + floorHeight / 2),
                Shape = new RectangleShape2D
                {
                    Size = new Vector2(pixelSize.X, floorHeight)
                }
            };

            area.AddChild(collisionShape);
            AddChild(area);
            return area;
        }

        return null;
    }
}

public class AreaData
{
    public bool Detected { get; set; }
    public int BodyCount { get; set; }
}

public enum AreaType
{
    Floor,
    WallLeft,
    WallRight
}
