namespace Template;

public abstract partial class Character : CharacterBody2D
{
    public EntityComponent EntityComponent { get; private set; }

    public override void _Ready()
    {
        EntityComponent = this.GetNode<EntityComponent>();
        EntityComponent.SwitchState(Idle());
        Init();
    }

    public override void _PhysicsProcess(double delta)
    {
        MoveAndSlide();
        Update(delta);
    }

    public virtual void Init() { }
    public virtual void Update(double delta) { }
    public virtual void IdleState(State state) { }

    protected State Idle()
    {
        State state = new(nameof(Idle));

        IdleState(state);

        return state;
    }
}
