using CSharpUtils;
using Godot;

namespace Template;

[GlobalClass, Icon("res://Template/Sprites/Icons/Gear/gear.svg")]
public partial class EntityComponent : Node
{
    State curState;

    public override void _Ready()
    {
        curState = new State("Undefined");

        PuddleReflectionUtils.CreateReflection(GetOwner());
    }

    public override void _PhysicsProcess(double d)
    {
        float delta = (float)d;
        curState.Update(delta);
    }

    public void SwitchState(State newState)
    {
        curState.Exit();
        newState.Enter();
        curState = newState;
    }

    public bool IsState(string state) => curState.ToString() == state.ToLower();
}

