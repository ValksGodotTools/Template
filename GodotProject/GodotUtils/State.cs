namespace GodotUtils;

using System;

public class State
{
    public Action Enter { get; set; } = () => { };
    public Action<float> Update { get; set; } = delta => { };
    public Action Transitions { get; set; } = () => { };
    public Action Exit { get; set; } = () => { };

    string name;

    public State(string name = "")
    {
        this.name = name;
    }

    public override string ToString() => name.ToLower();
}
