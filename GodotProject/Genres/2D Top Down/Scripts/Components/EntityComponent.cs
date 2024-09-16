using CSharpUtils;
using Godot;

namespace Template;

[GlobalClass, Icon("res://Template/Sprites/Icons/Gear/gear.svg")]
[Visualize(nameof(_curState))]
public partial class EntityComponent : Node
{
    State _curState;

    public override void _Ready()
    {
        _curState = new State("Undefined");

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
        newState.Enter();
        _curState = newState;
    }

    public bool IsState(string state)
    {
        return string.Equals(_curState.ToString(), state, System.StringComparison.OrdinalIgnoreCase);
    }
}

