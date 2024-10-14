using Godot;
using GodotUtils;

namespace Template.FPS3D;

[SceneTree]
public partial class SpaceShip : Node3D, IStateMachine
{
    private State _curState;
    private CharacterBody3D _body;
    private Vector3 _velocity;
    
    public override void _Ready()
    {
        _curState = Dorment();
        _body = _.CharacterBody3D;
        TakeControlOfShip();
    }

    public override void _PhysicsProcess(double delta)
    {
        _curState.Update((float)delta);
    }

    public void SwitchState(State newState)
    {
        _curState.Exit();
        _curState = newState;
        _curState.Enter();
    }
    
    private State Dorment()
    {
        State state = new(nameof(Dorment));

        return state;
    }
    
    public State Active()
    {
        State state = new(nameof(Active))
        {
            Update = delta =>
            {
                if (Input.IsActionJustPressed(InputActions.SpaceshipTakeoff))
                {
                    SwitchState(AnimateTakeOff());
                }
            }
        };

        return state;
    }
    
    public State AnimateTakeOff()
    {
        State state = new(nameof(AnimateTakeOff))
        {
            Enter = () =>
            {
                new GTween(this)
                    .Animate("position", Position + Vector3.Up * 3, 2).EaseIn()
                    .Animate("position", Position + Vector3.Up * 20, 3).EaseOut()
                    .Callback(() => SwitchState(Flight()));
            }
        };

        return state;
    }
    
    public State Flight()
    {
        State state = new(nameof(Flight))
        {
            Enter = () => 
            {
                _body.GetNode<CollisionShape3D>().SetDeferred(nameof(CollisionShape3D
                    .PropertyName.Disabled), false);
            },
            
            Update = delta =>
            {
                if (Input.IsActionPressed(InputActions.MoveUp))
                {
                    _velocity += Vector3.Forward;
                }
                
                if (Input.IsActionPressed(InputActions.MoveDown))
                {
                    _velocity += Vector3.Back;
                }

                _body.Velocity = _velocity;
            }
        };

        return state;
    }
    
    private void TakeControlOfShip()
    {
        _.Area3D.Get().BodyEntered += body =>
        {
            if (body is Player player)
            {
                SwitchState(Active());
                player.QueueFree();
                _.Camera3D.MakeCurrent();
            }
        };
    }
}
