namespace Template;

public abstract class State
{
    public Entity Entity { get; set; }

    public void Switch(State newState)
    {
        Entity.CurState.Exit();
        Entity.CurState = newState;
        Entity.CurState.Enter();
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }

    public override string ToString()
    {
        // This will turn "PlayerStateIdle" into "Idle"
        var name = GetType().Name;
        return name.Substring(name.IndexOf("State") + "State".Length);
    }
}
