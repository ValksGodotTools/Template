using System;

namespace GodotUtils;

public class State(string name = "")
{
    public Action Enter { get; set; } = () => { };
    public Action<float> Update { get; set; } = delta => { };
    public Action Transitions { get; set; } = () => { };
    public Action Exit { get; set; } = () => { };

    private string _name = name;

    public override string ToString()
    {
        return _name.ToLower();
    }
}
