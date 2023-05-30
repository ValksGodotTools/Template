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
    readonly Dictionary<string, RayCast2D[]> rayCasts = new();

    bool isFloor => areaDataFloor.Detected;
    bool isWallLeft => areaDataWallLeft.Detected;
    bool isWallRight => areaDataWallRight.Detected;

    // These may be used later on. For example temporarily disabling the collision
    // mask while jumping over a one-way platform
    Area2D areaFloor;
    Area2D areaWallLeft;
    Area2D areaWallRight;

    int areaFloorBodyCount;
    int areaWallLeftBodyCount;
    int areaWallRightBodyCount;

    AreaData areaDataFloor = new();
    AreaData areaDataWallLeft = new();
    AreaData areaDataWallRight = new();

    public override void _Ready()
    {
        MotionMode = MotionModeEnum.Grounded;

        areaFloor = SetupArea("Floor", areaDataFloor);
        areaWallLeft = SetupArea("WallLeft", areaDataWallLeft);
        areaWallRight = SetupArea("WallRight", areaDataWallRight);
    }

    public override void _PhysicsProcess(double delta)
    {
        var horzDir = Input.GetAxis("move_left", "move_right");
        var vel = Velocity;

        // Horizontal movement
        vel.X += horzDir * acceleration;
        vel.X = Utils.ClampAndDampen(vel.X, friction, maxSpeed);

        // Jump
        if (Input.IsActionJustPressed("jump") && isFloor)
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

    Area2D SetupArea(string name, AreaData areaData)
    {
        var area = GetNode<Area2D>($"Areas/{name}");

        area.BodyEntered += body =>
        {
            if (body is not Player)
            {
                areaData.Detected = true;
                areaData.BodyCount++;
            }
        };

        area.BodyExited += body =>
        {
            if (body is not Player)
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
