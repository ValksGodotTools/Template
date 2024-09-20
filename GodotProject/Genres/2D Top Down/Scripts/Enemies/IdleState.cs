using Godot;
using GodotUtils;

namespace Template.TopDown2D;

[GlobalClass]
public partial class IdleState : EnemyState
{
    [Export] private EnemyState _detectPlayerState;
    [Export] private EnemyState _idleActionState;
    [Export] private Area2D _playerDetectArea;

    private GTween _delayUntilSlide;
    private bool _isBodyEnteredSubscribed;

    protected override void Enter()
    {
        Sprite.PlayRandom("idle");

        GTween.Delay(this, 1, () =>
        {
            _isBodyEnteredSubscribed = true;
            _playerDetectArea.BodyEntered += BodyEnteredCallback;
            _playerDetectArea.SetDeferred(Area2D.PropertyName.Monitoring, true);

            _delayUntilSlide = GTween.Delay(this, 1, () =>
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
            if (body.IsInGroup("Player"))
            {
                EnemyComponent.Target = body;
                SwitchState(_detectPlayerState);
            }
        }
    }
}
