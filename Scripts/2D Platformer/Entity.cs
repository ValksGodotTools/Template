namespace Template.Platformer2D;

using System.Reflection;

public abstract partial class Entity : CharacterBody2D
{
    protected AnimatedSprite2D sprite;

    private Label stateLabel;
    private State curState;

    public override void _Ready()
    {
        sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        stateLabel = new Label();
        AddChild(stateLabel);

        Init();
        InitStates();

        curState = InitialState();
        UpdateStateLabel(curState);

        curState.Enter();
    }

    public override void _PhysicsProcess(double delta)
    {
        MoveAndSlide();

        Update();
        curState.Update();
        curState.Transitions();
    }

    protected abstract State InitialState();

    public void SwitchState(State newState)
    {
        curState.Exit();
        newState.Enter();
        curState = newState;

        UpdateStateLabel(newState);
    }

    public virtual void Init() { }
    public virtual void Update() { }

    void InitStates()
    {
        // Invoke any private methods starting with "State" in the entity class
        // For example "StateIdle()" or "StateJump()"
        // This is sort of living on the edge because this means no other private
        // methods can start with "State" in their names. I'm assuming no such
        // methods will be created in the future.
        foreach (var methodInfo in GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance))
            if (methodInfo.Name.StartsWith("State"))
                methodInfo.Invoke(this, null);
    }

    void UpdateStateLabel(State state)
    {
        stateLabel.Text = state.ToString();
        stateLabel.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.CenterBottom);
        stateLabel.Position -= new Vector2(0, stateLabel.Size.Y / 2);
    }
}
