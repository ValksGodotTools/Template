using CSharpUtils;
using Godot;

namespace Template.TopDown2D;

/// <summary>
/// This script assumes all enemies extend from RigidBody2D
/// </summary>
[GlobalClass]
public abstract partial class NodeState : Node2D
{
    [Export] protected NodeState NextState { get; private set; }

    public State State { get; private set; }

    protected Player Player { get => _entityComponent.Player; set => _entityComponent.Player = value; }
    protected RigidBody2D Entity { get; private set; }
    protected AnimatedSprite2D Sprite { get => _entityComponent.AnimatedSprite; }

    private EnemyComponent _entityComponent;

    public override void _Ready()
    {
        _entityComponent = GetParent<EnemyComponent>();

        Entity = GetOwner() as RigidBody2D;

        State = new State(GetType().Name.Replace("State", ""))
        {
            Enter = Enter,
            Update = Update,
            Exit = Exit
        };

        NextState ??= _entityComponent.IdleState;
    }
    
    protected virtual void Enter() { }
    protected virtual void Update(float delta) { }
    protected virtual void Exit() { }

    protected bool IsState(string state)
    {
        return _entityComponent.IsState(state);
    }

    protected void SwitchState(NodeState state)
    {
        _entityComponent.SwitchState(state);
    }
}
