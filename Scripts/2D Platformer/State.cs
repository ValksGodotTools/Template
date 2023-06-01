namespace Template.Platformer2D;

public class State
{
    public Action Enter { get; set; } = () => { };
    public Action Update { get; set; } = () => { };
    public Action Transitions { get; set; } = () => { };
    public Action Exit { get; set; } = () => { };

    private Entity entity;
    private string name;

    public State(Entity entity, string name = "")
    {
        this.entity = entity;
        this.name = name;
    }

    public override string ToString() => name;
}
