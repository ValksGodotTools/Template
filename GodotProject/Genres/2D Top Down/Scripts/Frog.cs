using Template.TopDown2D;

namespace Template;

public partial class Frog : Character
{
    [Export] AnimatedSprite2D animatedSprite;
    [Export] Area2D area;

    Player player;
    Vector2 landTarget;

    public override void Init()
    {
        area.SetDeferred(Area2D.PropertyName.Monitoring, false);
        area.BodyEntered += body =>
        {
            if (EntityComponent.IsState("idle"))
            {
                if (body is Player player)
                {
                    this.player = player;
                    EntityComponent.SwitchState(PreJump());
                }
            }
        };
    }

    public override void IdleState(State state)
    {
        state.Enter = () =>
        {
            animatedSprite.Play("idle");

            GTween.Delay(this, 1, () =>
            {
                area.SetDeferred(Area2D.PropertyName.Monitoring, true);
            });
        };

        state.Exit = () =>
        {
            area.SetDeferred(Area2D.PropertyName.Monitoring, false);
        };
    }

    State PreJump()
    {
        GTween tweenShake = new(animatedSprite);
        Vector2 prevSpritePos = Vector2.Zero;

        State state = new(nameof(PreJump))
        {
            Enter = () =>
            {
                prevSpritePos = animatedSprite.Position;
                animatedSprite.Play("pre_jump");

                Vector2 shake_offset = new(2, 0.5f);
                double shake_duration = 0.05;

                tweenShake.SetAnimatingProp(Node2D.PropertyName.Position)
                    .AnimateProp(animatedSprite.Position - shake_offset, shake_duration)
                    .AnimateProp(animatedSprite.Position + shake_offset, shake_duration)
                    .Loop();

                GTween.Delay(this, 1, () =>
                {
                    EntityComponent.SwitchState(Jump());
                });
            },

            Exit = () =>
            {
                animatedSprite.Position = prevSpritePos;
                landTarget = player.Position;
                tweenShake.Stop();
            }
        };

        return state;
    }

    State Jump()
    {
        State state = new(nameof(Jump))
        {
            Enter = () =>
            {
                animatedSprite.Play("jump");

                CollisionLayer = (uint)GMath.GetLayerValues(3);

                double jump_time = 1.0;

                new GTween(this)
                    .Animate(Node2D.PropertyName.Position, player.Position, jump_time)
                    .EaseIn();

                new GTween(animatedSprite)
                    .SetAnimatingProp(Node2D.PropertyName.Scale)
                    .AnimateProp(animatedSprite.Scale * new Vector2(4, 2), 0.2).EaseOut()
                    .AnimateProp(animatedSprite.Scale * new Vector2(2, 4), jump_time * 0.5).EaseOut()
                    .AnimateProp(animatedSprite.Scale * new Vector2(1.5f, 0.5f), jump_time * 0.5).EaseIn()
                    .Callback(() => animatedSprite.Play("idle"))
                    .AnimateProp(animatedSprite.Scale * Vector2.One, jump_time * 0.5)
                    .Callback(() => EntityComponent.SwitchState(Idle()));
            },

            Exit = () =>
            {
                CollisionLayer = (uint)GMath.GetLayerValues(2);
            }
        };

        return state;
    }
}
