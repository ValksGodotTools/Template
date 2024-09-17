using CSharpUtils;
using Godot;
using GodotUtils;

namespace Template;

[GlobalClass]
public partial class SlideState : NodeState
{
    [Export] private AnimatedSprite2D _animatedSprite;
    [Export] private NodeState _idleState;

    public override State GetState()
    {
        State state = new("Slide")
        {
            Enter = () =>
            {
                Vector2 target = Entity.Position + GMath.RandDir() * 100;
                Vector2 force = (target - Entity.Position) * 3;

                Entity.ApplyCentralImpulse(force);

                new GTween(_animatedSprite)
                    .SetAnimatingProp(Node2D.PropertyName.Scale)
                    .AnimateProp(_animatedSprite.Scale * new Vector2(1.3f, 0.5f), 0.2).EaseOut()
                    .AnimateProp(_animatedSprite.Scale * new Vector2(1, 1), 0.3).EaseOut()
                    .Callback(() => SwitchState(_idleState));
            }
        };

        return state;
    }
}
