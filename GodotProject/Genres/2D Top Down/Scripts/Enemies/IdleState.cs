using CSharpUtils;
using Godot;
using GodotUtils;
using Template.TopDown2D;

namespace Template;

[GlobalClass]
public partial class IdleState : NodeState
{
    [Export] private AnimatedSprite2D _animatedSprite;
    [Export] private NodeState _idleActionState;
    [Export] private NodeState _detectPlayerState;
    [Export] private Area2D _playerDetectArea;

    private EntityComponent _entityComponent;
    private RigidBody2D _entity;
    private bool _isBodyEnteredSubscribed;

    public override void _Ready()
    {
        _entityComponent = GetParent<EntityComponent>();
        _entity = GetOwner() as RigidBody2D;
    }

    public override State GetState()
    {
        GTween delayUntilSlide = null;

        State state = new("Idle")
        {
            Enter = () =>
            {
                _animatedSprite.PlayRandom("idle");

                GTween.Delay(this, 1, () =>
                {
                    _isBodyEnteredSubscribed = true;
                    _playerDetectArea.BodyEntered += BodyEnteredCallback;
                    _playerDetectArea.SetDeferred(Area2D.PropertyName.Monitoring, true);

                    delayUntilSlide = GTween.Delay(this, 1, () =>
                    {
                        _entityComponent.SwitchState(_idleActionState);
                    });
                });
            },
            Exit = () =>
            {
                delayUntilSlide?.Stop();

                if (_isBodyEnteredSubscribed)
                {
                    _isBodyEnteredSubscribed = false;
                    _playerDetectArea.BodyEntered -= BodyEnteredCallback;
                }

                _playerDetectArea.SetDeferred(Area2D.PropertyName.Monitoring, false);
            }
        };

        return state;
    }

    private void BodyEnteredCallback(Node2D body)
    {
        if (_entityComponent.IsState("Idle"))
        {
            if (body is Player player)
            {
                _detectPlayerState.Player = player;
                _entityComponent.SwitchState(_detectPlayerState);
            }
        }
    }
}
