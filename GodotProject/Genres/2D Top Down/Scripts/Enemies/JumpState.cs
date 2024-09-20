using Godot;
using GodotUtils;

namespace Template.TopDown2D;

[GlobalClass]
public partial class JumpState : NodeState
{
    protected override void Enter()
    {
        if (!IsInstanceValid(EnemyComponent.Target))
        {
            SwitchState(NextState, false);
            return;
        }

        Sprite.Play("jump");

        Entity.SetCollisionLayerAndMask(3);

        Vector2 force = (EnemyComponent.Target.Position - Entity.Position) * 2;

        Entity.ApplyCentralImpulse(force);

        double jump_time = 1.0;

        new GTween(Sprite)
            .SetAnimatingProp(Node2D.PropertyName.Scale)
            .AnimateProp(Sprite.Scale * new Vector2(4, 2), 0.2).EaseOut()
            .AnimateProp(Sprite.Scale * new Vector2(2, 4), jump_time * 0.5).EaseOut()
            .AnimateProp(Sprite.Scale * new Vector2(1.5f, 0.5f), jump_time * 0.5).EaseIn()
            .Callback(() =>
            {
                Entity.LinearVelocity = Vector2.Zero;
                Sprite.Play("idle");
                Entity.SetCollisionLayerAndMask(2);
            })
            .AnimateProp(Sprite.Scale * Vector2.One, jump_time * 0.5)
            .Callback(() => SwitchState(NextState));
    }
}
