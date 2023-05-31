namespace Template;

public abstract partial class Entity : CharacterBody2D
{
    public AnimatedSprite2D Sprite { get; private set; }
    public State CurState { get; set; }

    public override void _Ready()
    {
        Sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        Init();
        CurState.Enter();
    }

    public override void _PhysicsProcess(double delta)
    {
        MoveAndSlide();

        Update();
        CurState.Update();
    }

    public virtual void Init() { }
    public virtual void Update() { }
}
