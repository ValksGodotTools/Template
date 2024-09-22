using Godot;
using Template.Netcode.Client;

namespace Template.TopDown2D;

public partial class Player : CharacterBody2D
{
    [Export] private PlayerConfig _config;

    private PlayerMoveManager _moveManager;
    private PlayerLookManager _lookManager;
    private PlayerDashManager _dashManager;

    private CameraShakeComponent _cameraShake;
    private Vector2 _prevPosition;
    private Vector2 _moveDirection;
    private Vector2 _externalForce;
    private ENetClient _client;
    private AnimatedSprite2D _sprite;
    private Sprite2D _cursor;

    [OnInstantiate]
    private void Init()
    {

    }

    public override void _Ready()
    {
        _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _cursor = GetNode<Sprite2D>("Cursor");
        _cameraShake = GetTree().Root.GetNode<CameraShakeComponent>("Level/Camera2D/CameraShake");
        _client = ServiceProvider.Services.Get<UINetControlPanel>().Net.Client;

        _dashManager = new PlayerDashManager(_config, _sprite);
        _moveManager = new PlayerMoveManager(_config);
        _lookManager = new PlayerLookManager();

        _dashManager.ResetDashState();
    }

    public override void _PhysicsProcess(double delta)
    {
        MoveAndSlide();

        _moveManager.HandleMovement(this, out Vector2 moveDirection);
        _moveDirection = moveDirection;

        _dashManager.HandleDash(this, _moveDirection);
        _lookManager.HandleLookDirection(_cursor, this, _config, delta);
        _lookManager.UpdateControllerLookInputs(delta);

        // Apply external force to the player's velocity
        Velocity += _externalForce;

        // Gradually reduce the external force over time
        _externalForce = _externalForce.Lerp(Vector2.Zero, _config.ExternalForceDecay);
    }

    public void ApplyExternalForce(Vector2 force)
    {
        _externalForce += force;
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
