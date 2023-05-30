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

    bool isFloor;
    bool isWallLeft;
    bool isWallRight;

    // These may be used later on. For example temporarily disabling the collision
    // mask while jumping over a one-way platform
    Area2D areaFloor;
    Area2D areaWallLeft;
    Area2D areaWallRight;

    int areaFloorBodyCount;
    int areaWallLeftBodyCount;
    int areaWallRightBodyCount;

    public override void _Ready()
    {
        MotionMode = MotionModeEnum.Grounded;

        SetupAreaFloor();
        SetupAreaWallLeft();
        SetupAreaWallRight();
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

    void SetupAreaFloor()
    {
        areaFloor = GetNode<Area2D>("Areas/Floor");
        areaFloor.BodyEntered += body =>
        {
            if (body is not Player)
            {
                isFloor = true;
                areaFloorBodyCount++;
            }
        };
        areaFloor.BodyExited += body =>
        {
            if (body is not Player)
            {
                areaFloorBodyCount--;
                if (areaFloorBodyCount == 0)
                    isFloor = false;
            }
        };
    }

    void SetupAreaWallLeft()
    {
        areaWallLeft = GetNode<Area2D>("Areas/WallLeft");
        areaWallLeft.BodyEntered += body =>
        {
            if (body is not Player)
            {
                isWallLeft = true;
                areaWallLeftBodyCount++;
            }
        };
        areaWallLeft.BodyExited += body =>
        {
            if (body is not Player)
            {
                areaWallLeftBodyCount--;
                if (areaWallLeftBodyCount == 0)
                    isWallLeft = false;
            }
        };
    }

    void SetupAreaWallRight()
    {
        areaWallRight = GetNode<Area2D>("Areas/WallRight");
        areaWallRight.BodyEntered += body =>
        {
            if (body is not Player)
            {
                isWallRight = true;
                areaWallRightBodyCount++;
            }
        };
        areaWallRight.BodyExited += body =>
        {
            if (body is not Player)
            {
                areaWallRightBodyCount--;
                if (areaWallRightBodyCount == 0)
                    isWallRight = false;
            }
        };
    }
}
