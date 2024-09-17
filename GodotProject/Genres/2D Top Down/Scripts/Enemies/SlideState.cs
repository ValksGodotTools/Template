using CSharpUtils;
using Godot;
using GodotUtils;

namespace Template;

[GlobalClass]
public partial class SlideState : NodeState
{
    [Export] private AnimatedSprite2D _animatedSprite;
    [Export] private NodeState _idleState;

    private EntityComponent _entityComponent;
    private RigidBody2D _entity;

    public override void _Ready()
    {
        _entityComponent = GetParent<EntityComponent>();
        _entity = GetOwner() as RigidBody2D;
    }

    public override State GetState()
    {
        State state = new("Slide")
        {
            Enter = () =>
            {
                Vector2 target = _entity.Position + GMath.RandDir() * 100;
                Vector2 force = (target - _entity.Position) * 3;

                _entity.ApplyCentralImpulse(force);

                new GTween(_animatedSprite)
                    .SetAnimatingProp(Node2D.PropertyName.Scale)
                    .AnimateProp(_animatedSprite.Scale * new Vector2(1.3f, 0.5f), 0.2).EaseOut()
                    .AnimateProp(_animatedSprite.Scale * new Vector2(1, 1), 0.3).EaseOut()
                    .Callback(() => _entityComponent.SwitchState(_idleState));
            }
        };

        return state;
    }
}
