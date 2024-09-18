using CSharpUtils;
using Godot;

namespace Template.TopDown2D;

/// <summary>
/// This script assumes all enemies are of type Node2D and have animated sprite nodes.
/// </summary>
[GlobalClass, Icon("res://Template/Sprites/Icons/Gear/gear.svg")]
[Visualize(nameof(_curState))]
public partial class EnemyComponent : Node
{
    [Export] private EnemyConfig _config;
    [Export] public AnimatedSprite2D AnimatedSprite { get; private set; }
    [Export] public NodeState IdleState { get; private set; }

    public Player Player { get; set; }

    private State _curState;
    private Node2D _enemy;

    public override void _Ready()
    {
        _enemy = GetOwner<Node2D>();

        SetupState();
        SetMaterial();
        CreateReflection();
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
        _curState = newState.State;
        _curState.Enter();
    }

    public bool IsState(string state)
    {
        return string.Equals(_curState.ToString(), state, System.StringComparison.OrdinalIgnoreCase);
    }

    private void SetupState()
    {
        if (IdleState != null)
        {
            _curState = IdleState.State;
            _curState.Enter();
        }
    }

    private void SetMaterial()
    {
        AnimatedSprite.SelfModulate = _config.Color;

        AnimatedSprite.Material ??= new CanvasItemMaterial()
        {
            BlendMode = _config.BlendMode,
            LightMode = _config.LightMode
        };
    }

    private void CreateReflection()
    {
        PuddleReflectionUtils.CreateReflection(_enemy);
    }
}

