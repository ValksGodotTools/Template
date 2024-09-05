using CSharpUtils;

namespace Template;

public interface IBaseEntity
{
    public EntityComponent EntityComponent { get; set; }

    public void IdleState(State state);
}

