using Godot;
using GodotUtils;

namespace Template.TopDown2D;

[GlobalClass]
public partial class IdleState : NodeState
{
    [Export] private NodeState _detectPlayerState;
    [Export] private NodeState _idleActionState;
    [Export] private Area2D _playerDetectArea;

    GTween delayUntilSlide = null;
    private bool _isBodyEnteredSubscribed;

    protected override void Enter()
    {
        Sprite.PlayRandom("idle");

        GTween.Delay(this, 1, () =>
        {
            _isBodyEnteredSubscribed = true;
            _playerDetectArea.BodyEntered += BodyEnteredCallback;
            _playerDetectArea.SetDeferred(Area2D.PropertyName.Monitoring, true);

            delayUntilSlide = GTween.Delay(this, 1, () =>
            {
                SwitchState(_idleActionState);
            });
        });
    }

    protected override void Exit()
    {
        delayUntilSlide?.Stop();

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
            if (body is Player player)
            {
                Player = player;
                SwitchState(_detectPlayerState);
            }
        }
    }
}
