using Godot;
using GodotUtils;

namespace Template.TopDown2D;

[GlobalClass]
public partial class JumpState : EnemyState
{
    [Export] private double _jumpTime = 1.0;
    [Export] private string _jumpAnimationName = "jump";
    [Export] private string _idleAnimationName = "idle";

    protected override void Enter()
    {
        if (!IsInstanceValid(EnemyComponent.Target))
        {
            SwitchState(NextState, false);
            return;
        }

        Sprite.Play(_jumpAnimationName);

        Vector2 force = (EnemyComponent.Target.Position - Entity.Position) * 2;

        Entity.ApplyCentralImpulse(force);

        new GTween(Sprite)
            .SetAnimatingProp(Node2D.PropertyName.Scale)
            .AnimateProp(Sprite.Scale * new Vector2(4, 2), 0.2).EaseOut()
            .AnimateProp(Sprite.Scale * new Vector2(2, 4), _jumpTime * 0.5).EaseOut()
            .AnimateProp(Sprite.Scale * new Vector2(1.5f, 0.5f), _jumpTime * 0.5).EaseIn()
            .Callback(() =>
            {
                Entity.LinearVelocity = Vector2.Zero;
                Sprite.Play(_idleAnimationName);
            })
            .AnimateProp(Sprite.Scale * Vector2.One, _jumpTime * 0.5)
            .Callback(() => SwitchState(NextState));
    }
}
