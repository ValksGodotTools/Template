using CSharpUtils;
using Godot;
using GodotUtils;

namespace Template.TopDown2D;

[GlobalClass, Icon("res://Template/Sprites/Icons/Gear/gear.svg")]
[Visualize(nameof(_curState))]
public partial class EntityComponent : Node
{
    [Export] public AnimatedSprite2D AnimatedSprite { get; private set; }

    [Export] private NodeState _idleActionState;
    [Export] private NodeState _detectPlayerState;
    [Export] private Area2D _playerDetectArea;

    private State _curState;
    private bool _isBodyEnteredSubscribed;

    public override void _Ready()
    {
        _curState = Idle();
        _curState.Enter();

        PuddleReflectionUtils.CreateReflection(GetOwner());
    }

    public override void _PhysicsProcess(double d)
    {
        float delta = (float)d;
        _curState.Update(delta);
    }

    public void SwitchState(State newState)
    {
        _curState.Exit();
        _curState = newState;
        _curState.Enter();
    }

    public void SwitchState(NodeState newState)
    {
        _curState.Exit();
        _curState = newState.GetState();
        _curState.Enter();
    }

    public bool IsState(string state)
    {
        return string.Equals(_curState.ToString(), state, System.StringComparison.OrdinalIgnoreCase);
    }

    public State Idle()
    {
        GTween delayUntilSlide = null;

        State state = new("Idle")
        {
            Enter = () =>
            {
                AnimatedSprite.PlayRandom("idle");

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
        if (IsState("Idle"))
        {
            if (body is Player player)
            {
                _detectPlayerState.Player = player;
                SwitchState(_detectPlayerState);
            }
        }
    }
}

