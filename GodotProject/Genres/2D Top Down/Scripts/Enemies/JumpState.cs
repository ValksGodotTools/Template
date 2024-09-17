using CSharpUtils;
using Godot;
using GodotUtils;

namespace Template.TopDown2D;

[GlobalClass]
public partial class JumpState : NodeState
{
    [Export] private Area2D _playerDetectArea;

    public override State GetState()
    {
        GTween tweenShake = new(Sprite);
        Vector2 prevSpritePos = Vector2.Zero;

        State state = new("Pre Jump")
        {
            Enter = () =>
            {
                prevSpritePos = Sprite.Position;
                Sprite.Play("pre_jump");

                Vector2 shake_offset = new(2, 0.5f);
                double shake_duration = 0.05;

                tweenShake.SetAnimatingProp(Node2D.PropertyName.Position)
                    .AnimateProp(Sprite.Position - shake_offset, shake_duration)
                    .AnimateProp(Sprite.Position + shake_offset, shake_duration)
                    .Loop();

                GTween.Delay(this, 1, () =>
                {
                    SwitchState(Jump());
                });
            },

            Exit = () =>
            {
                Sprite.Position = prevSpritePos;
                tweenShake.Stop();
            }
        };

        return state;
    }

    private State Jump()
    {
        State state = new(nameof(Jump))
        {
            Enter = () =>
            {
                Sprite.Play("jump");

                Entity.SetCollisionLayerAndMask(3);

                Vector2 force = (Player.Position - Entity.Position) * 2;

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
                    .Callback(() => SwitchState(IdleState));
            }
        };

        return state;
    }
}
