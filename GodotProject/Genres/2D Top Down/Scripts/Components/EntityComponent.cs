using CSharpUtils;
using Godot;

namespace Template.TopDown2D;

[GlobalClass, Icon("res://Template/Sprites/Icons/Gear/gear.svg")]
[Visualize(nameof(_curState))]
public partial class EntityComponent : Node
{
    private State _curState;

    public override void _Ready()
    {
        _curState = new State("Undefined");

        IdleState idleState = GetNodeOrNull<IdleState>("IdleState");
        
        if (idleState != null)
        {
            _curState = idleState.GetState();
            _curState.Enter();
        }

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
}

