using CSharpUtils;
using Godot;

namespace Template.TopDown2D;

[GlobalClass, Icon("res://Template/Sprites/Icons/Gear/gear.svg")]
public partial class EntityComponent : Node2D
{
    [Export] private EntityConfig _config;
    [Export] public AnimatedSprite2D AnimatedSprite { get; private set; }
    [Export] public NodeState IdleState { get; private set; }

    protected State _curState;
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

    public virtual void SwitchState(NodeState newState)
    {
        SwitchState(newState.State);
    }

    public bool IsState(string state)
    {
        return string.Equals(_curState.ToString(), state, System.StringComparison.OrdinalIgnoreCase);
    }

    private void SwitchState(State newState)
    {
        _curState.Exit();
        _curState = newState;
        _curState.Enter();
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
