using CSharpUtils;
using Godot;

namespace Template.TopDown2D;

public abstract partial class NodeState : Node
{
    public Player Player { get; set; }

    public abstract State GetState();

    protected RigidBody2D Entity { get; private set; }

    private EntityComponent _entityComponent;

    public override void _Ready()
    {
        _entityComponent = GetParent<EntityComponent>();
        Entity = GetOwner() as RigidBody2D;
    }

    protected bool IsState(string state)
    {
        return _entityComponent.IsState(state);
    }

    protected void SwitchState(State state)
    {
        _entityComponent.SwitchState(state);
    }

    protected void SwitchState(NodeState state)
    {
        _entityComponent.SwitchState(state);
    }
}
