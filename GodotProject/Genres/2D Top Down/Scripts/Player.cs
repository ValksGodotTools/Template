namespace Template.TopDown2D;

public partial class Player : Character, INetPlayer
{
    #region Config

    float speed = 50;
    float friction = 0.2f;
    float dashStrength = 1500;

    #endregion

    #region Variables

    CameraShake cameraShake;
    Vector2 prevPosition;
    Vector2 direction;
    Sprite2D sprite;
    Net net;

    bool canDash;

    #endregion

    public override void Init()
    {
        canDash = true;
        net = Global.Services.Get<Net>();
        sprite = GetNode<Sprite2D>("Sprite2D");
        cameraShake = GetTree().Root.GetNode<CameraShake>("Level/Camera2D/CameraShake");
    }

    public override void Update(double delta)
    {
        direction = GodotUtils.World2D.TopDown.Utils.GetMovementInput();

        // Velocity is mutiplied by delta for us already
        Velocity += direction * speed;
        Velocity = Velocity.Lerp(Vector2.Zero, friction);

        if (Input.IsActionJustPressed("dash") && canDash && direction != Vector2.Zero)
        {
            Dash();
            canDash = false;
            GTween.Delay(this, 0.2, () => canDash = true);
        }
    }

    public void Dash()
    {
        GTween ghosts = new GTween(this)
            .Delay(0.0565) //0.0565 for exactly 3 ghosts at perfect spacing
            .Callback(AddGhost)
            .Loop();

        new GTween(this)
            .Animate(CharacterBody2D.PropertyName.Velocity, direction * dashStrength, 0.1)
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
            net.Client.Send(new CPacketPosition
            {
                Position = Position
            });
        }

        prevPosition = Position;
    }
}
