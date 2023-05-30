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

        // Jump
        if (Input.IsActionJustPressed("jump") && IsColliding(AreaType.Floor))
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
            areas[type] = (SetupArea(type + "", areaData), areaData);
        }
    }

    Area2D SetupArea(string name, AreaData areaData)
    {
        var area = GetNode<Area2D>($"Areas/{name}");

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
