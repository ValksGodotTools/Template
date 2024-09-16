using Godot;
using GodotUtils;

namespace Template.TopDown2D;

[Visualize(nameof(Position), nameof(Velocity))]
public partial class Player : CharacterBody2D, INetPlayer
{
    [Visualize] [Export] private PlayerResource _config;

    private CameraShakeComponent _cameraShake;
    private Vector2 _prevPosition;
    private Vector2 _moveDirection;
    private GameClient _client;
    private Sprite2D _sprite;
    private Sprite2D _cursor;

    private double _controllerLookInputsActiveBuffer;
    private bool _canDash;
    private Vector2 _targetLookDirection;
    private Vector2 _currentLookDirection;
    private EntityComponent _entityComponent;

    public override void _Ready()
    {
        InitializeComponents();
        ResetDashState();
    }

    private void InitializeComponents()
    {
        _entityComponent = this.GetNode<EntityComponent>();
        _client = Game.Net.Client;
        _sprite = GetNode<Sprite2D>("Sprite2D");
        _cursor = GetNode<Sprite2D>("Cursor");
        _cameraShake = GetTree().Root.GetNode<CameraShakeComponent>("Level/Camera2D/CameraShake");
    }

    private void ResetDashState()
    {
        _canDash = true;
    }

    public override void _PhysicsProcess(double delta)
    {
        MoveAndSlide();
        HandleMovement();
        HandleDash();
        HandleLookDirection(delta);
        UpdateControllerLookInputs(delta);
    }

    private void HandleMovement()
    {
        Vector2 moveDirection = GetMoveDirection();
        _moveDirection = moveDirection;

        Velocity += moveDirection * _config.Speed;
        Velocity = Velocity.Lerp(Vector2.Zero, _config.Friction);
    }

    private static Vector2 GetMoveDirection()
    {
        Vector2 moveDirection = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        moveDirection += new Vector2(Input.GetJoyAxis(0, JoyAxis.LeftX), Input.GetJoyAxis(0, JoyAxis.LeftY));
        return moveDirection.Normalized();
    }

    private void HandleDash()
    {
        bool dashJustPressed = Input.IsActionJustPressed("dash") || Input.IsJoyButtonPressed(0, JoyButton.LeftShoulder);

        if (dashJustPressed && _canDash && _moveDirection != Vector2.Zero)
        {
            PerformDash();
            ResetDashStateAfterDelay();
        }
    }

    private void PerformDash()
    {
        Dash();
        _canDash = false;
    }

    private void ResetDashStateAfterDelay()
    {
        GTween.Delay(this, 0.2, () => _canDash = true);
    }

    private void HandleLookDirection(double delta)
    {
        _targetLookDirection = GetLookDirection();
        _currentLookDirection = _currentLookDirection.Slerp(_targetLookDirection, (float)(_config.LookLerpSpeed * delta));
        _cursor.LookAt(Position + _currentLookDirection.Normalized() * 100);
    }

    private Vector2 GetLookDirection()
    {
        if (_controllerLookInputsActiveBuffer > 0)
        {
            return GetControllerLookDirection();
        }
        else
        {
            return GetMouseLookDirection();
        }
    }

    private static Vector2 GetControllerLookDirection()
    {
        return Input.GetJoyName(0) switch
        {
            "PS3 Controller" => new Vector2(Input.GetJoyAxis(0, JoyAxis.RightX), -(Input.GetJoyAxis(0, JoyAxis.TriggerLeft) - 0.5f) * 2),
            _ => new Vector2(Input.GetJoyAxis(0, JoyAxis.RightX), Input.GetJoyAxis(0, JoyAxis.RightY))
        };
    }

    private Vector2 GetMouseLookDirection()
    {
        return (GetGlobalMousePosition() - Position).Normalized();
    }

    private void UpdateControllerLookInputs(double delta)
    {
        float rightX = Input.GetJoyAxis(0, JoyAxis.RightX);
        float rightY = Input.GetJoyAxis(0, JoyAxis.RightY);
        float triggerLeft = Input.GetJoyName(0) == "PS3 Controller" ? -(Input.GetJoyAxis(0, JoyAxis.TriggerLeft) - 0.5f) * 2 : 0;

        float sum = Mathf.Abs(rightX) + Mathf.Abs(rightY) + Mathf.Abs(triggerLeft);

        if (sum > 0.3)
        {
            _controllerLookInputsActiveBuffer = 1;
        }

        _controllerLookInputsActiveBuffer -= delta;
    }

    private void Dash()
    {
        GTween ghosts = new GTween(this)
            .Delay(0.0565)
            .Callback(AddGhost)
            .Loop();

        new GTween(this)
            .Animate(CharacterBody2D.PropertyName.Velocity, _moveDirection * _config.DashStrength, 0.1)
            .Delay(0.1)
            .Callback(() => ghosts.Stop());
    }

    private void AddGhost()
    {
        PlayerDashGhost ghost = Game.LoadPrefab<PlayerDashGhost>(Prefab.PlayerDashGhost);
        ghost.Position = Position;
        ghost.AddChild(_sprite.Duplicate());
        GetTree().CurrentScene.AddChild(ghost);
    }

    public void NetSendPosition()
    {
        if (Position != _prevPosition)
        {
            _client.Send(new CPacketPosition { Position = Position });
            _prevPosition = Position;
        }
    }
}
