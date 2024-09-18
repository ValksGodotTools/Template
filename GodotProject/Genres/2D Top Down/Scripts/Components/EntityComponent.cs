using CSharpUtils;
using Godot;

namespace Template.TopDown2D;

[GlobalClass, Icon("res://Template/Sprites/Icons/Gear/gear.svg")]
[Visualize(nameof(_curState))]
public partial class EntityComponent : Node2D
{
    [Export] private EntityConfig _config;
    [Export] public AnimatedSprite2D AnimatedSprite { get; private set; }
    [Export] public NodeState IdleState { get; private set; }

    protected State _curState; // temporarily made protected for Visualize in EnemyComponent
    private Node2D _entity;

    public override void _Ready()
    {
        _entity = GetOwner<Node2D>();

        SetupState();
        SetMaterial();
        CreateReflection();
    }

    public override void _PhysicsProcess(double delta)
    {
        _curState.Update((float)delta);
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
        else
        {
            SetPhysicsProcess(false);
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
        PuddleReflectionUtils.CreateReflection(_entity, AnimatedSprite);
    }
}
