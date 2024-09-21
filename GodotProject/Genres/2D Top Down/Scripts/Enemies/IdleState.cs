using Godot;
using GodotUtils;

namespace Template.TopDown2D;

[GlobalClass]
public partial class IdleState : EnemyState
{
    [Export] private EnemyState _detectPlayerState;
    [Export] private EnemyState _idleActionState;
    [Export] private Area2D _playerDetectArea;
    [Export] private double _idleTime = 1;
    [Export] private double _delayUntilIdleActionState = 1;
    [Export] private string _animationName = "idle";

    private GTween _delayUntilSlide;
    private bool _isBodyEnteredSubscribed;

    protected override void Enter()
    {
        Sprite.PlayRandom(_animationName);

        GTween.Delay(this, _idleTime, () =>
        {
            _isBodyEnteredSubscribed = true;
            _playerDetectArea.BodyEntered += BodyEnteredCallback;
            _playerDetectArea.SetDeferred(Area2D.PropertyName.Monitoring, true);

            _delayUntilSlide = GTween.Delay(this, _delayUntilIdleActionState, () =>
            {
                SwitchState(_idleActionState);
            });
        });
    }

    protected override void Exit()
    {
        _delayUntilSlide?.Stop();

        if (_isBodyEnteredSubscribed)
        {
            _isBodyEnteredSubscribed = false;
            _playerDetectArea.BodyEntered -= BodyEnteredCallback;
        }

        _playerDetectArea.SetDeferred(Area2D.PropertyName.Monitoring, false);
    }

    private void BodyEnteredCallback(Node2D body)
    {
        if (IsState("Idle"))
        {
            if (body.HasNode<PlayerComponent>())
            {
                EnemyComponent.Target = body;
                SwitchState(_detectPlayerState);
            }
        }
    }
}
