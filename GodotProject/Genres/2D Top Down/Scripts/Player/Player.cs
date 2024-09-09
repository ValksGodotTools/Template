using Godot;
using GodotUtils;

namespace Template.TopDown2D;

public partial class Player : Character, INetPlayer
{
    #region Config

    float speed = 50;
    float friction = 0.2f;
    float dashStrength = 1500;

    #endregion

    #region Variables

    CameraShakeComponent cameraShake;
    Vector2 prevPosition;
    Vector2 moveDirection;
    GameClient client;
    Sprite2D sprite;
    Sprite2D cursor;
    double controllerLookInputsActiveBuffer;

    bool canDash;

    #endregion

    public override void _Ready()
    {
        base._Ready();
        canDash = true;
        client = Game.Net.Client;
        sprite = GetNode<Sprite2D>("Sprite2D");
        cursor = GetNode<Sprite2D>("Cursor");
        cameraShake = GetTree().Root.GetNode<CameraShakeComponent>("Level/Camera2D/CameraShake");
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        Vector2 moveDirection = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        
        moveDirection += new Vector2(Input.GetJoyAxis(0, JoyAxis.LeftX), Input.GetJoyAxis(0, JoyAxis.LeftY));
        
        moveDirection = moveDirection.Normalized();

        this.moveDirection = moveDirection;

        // Velocity is mutiplied by delta for us already
        Velocity += moveDirection * speed;
        Velocity = Velocity.Lerp(Vector2.Zero, friction);

        if (Input.IsActionJustPressed("dash") && canDash && moveDirection != Vector2.Zero)
        {
            Dash();
            canDash = false;
            GTween.Delay(this, 0.2, () => canDash = true);
        }

        Vector2 lookDirection = Vector2.Zero;

        if (controllerLookInputsActiveBuffer > 0)
        {
            lookDirection = Input.GetJoyName(0) switch
            {
                // My PS2 controller shows up as "PS3 Controller" when I do Input.GetJoyName(0)
                // My PS2 controller requires a specific implementation in order for it to work in Godot
                "PS3 Controller" => new Vector2(Input.GetJoyAxis(0, JoyAxis.RightX), -(Input.GetJoyAxis(0, JoyAxis.TriggerLeft) - 0.5f) * 2),

                // All other controllers
                _ => new Vector2(Input.GetJoyAxis(0, JoyAxis.RightX), Input.GetJoyAxis(0, JoyAxis.RightY))
            };
        }
        else
        {
            lookDirection = GetGlobalMousePosition() - Position;
        }

        cursor.LookAt(Position + lookDirection.Normalized() * 100);

        AreControllerLookInputsActive();
        controllerLookInputsActiveBuffer -= delta;
    }

    private void AreControllerLookInputsActive()
    {
        float rightX = Input.GetJoyAxis(0, JoyAxis.RightX);
        float rightY = Input.GetJoyAxis(0, JoyAxis.RightY);

        float triggerLeft = 0;
        
        if (Input.GetJoyName(0) == "PS3 Controller")
        {
            triggerLeft = -(Input.GetJoyAxis(0, JoyAxis.TriggerLeft) - 0.5f) * 2;
        }

        float sum = Mathf.Abs(rightX) + Mathf.Abs(rightY) + Mathf.Abs(triggerLeft);

        GD.Print(sum);

        if (sum > 0.3)
        {
            controllerLookInputsActiveBuffer = 1;
        }
    }

    public void Dash()
    {
        GTween ghosts = new GTween(this)
            .Delay(0.0565) //0.0565 for exactly 3 ghosts at perfect spacing
            .Callback(AddGhost)
            .Loop();

        new GTween(this)
            .Animate(CharacterBody2D.PropertyName.Velocity, moveDirection * dashStrength, 0.1)
            .Delay(0.1)
            .Callback(() => ghosts.Stop());
    }

    public void AddGhost()
    {
        PlayerDashGhost ghost = Game.LoadPrefab<PlayerDashGhost>(Prefab.PlayerDashGhost);

        ghost.Position = Position;
        ghost.AddChild(sprite.Duplicate());

        GetTree().CurrentScene.AddChild(ghost);
    }

    public void NetSendPosition()
    {
        if (Position != prevPosition)
        {
            client.Send(new CPacketPosition
            {
                Position = Position
            });
        }

        prevPosition = Position;
    }
}

