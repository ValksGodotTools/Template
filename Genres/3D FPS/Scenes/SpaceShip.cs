using Godot;
using GodotUtils;

namespace Template.FPS3D;

[SceneTree]
public partial class SpaceShip : Node3D, IStateMachine
{
    private State _curState;
    private Quaternion _quatYawPitch;
    private float _yaw;
    private float _pitch;
    private bool _isFlying;
    private Vector3 _updatedPosition;

    public override void _Ready()
    {
        _curState = Dorment();
        _quatYawPitch = Quaternion;
        _updatedPosition = Position;
        TakeControlOfShip();
    }
    
    public override void _PhysicsProcess(double delta)
    {
        _curState.Update((float)delta);

        Quaternion = _quatYawPitch;
    }

    public override void _Input(InputEvent @event)
    {
        if (_isFlying && @event is InputEventMouseMotion mouse)
        {
            float sensitivity = 0.01f;

            // Adjust yaw and pitch
            _yaw += -mouse.Relative.X * sensitivity;
            _pitch += -mouse.Relative.Y * sensitivity;

            // Rebuild the quaternion from yaw and pitch
            Quaternion yawQuat = new(Vector3.Up, _yaw);
            Quaternion pitchQuat = new(Vector3.Right, _pitch);

            _quatYawPitch = yawQuat * pitchQuat;
        }
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
                    .Animate("position", Position + Vector3.Up * 3, 1).EaseIn()
                    //.Animate("position", Position + Vector3.Up * 20, 3).EaseOut()
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
                _isFlying = true;
            },
            
            Update = delta =>
            {
                // Forward / horizontal inputs
                Vector2 fhInputs = Input.GetVector(InputActions.MoveLeft, InputActions.MoveRight, InputActions.MoveUp, InputActions.MoveDown);

                // Up / down inputs
                float udInputs = Input.GetAxis(InputActions.SpaceshipDown, InputActions.SpaceshipUp);

                _updatedPosition += Quaternion * new Vector3(fhInputs.X, udInputs, fhInputs.Y);

                Position = Position.Lerp(_updatedPosition, 0.1f);
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
