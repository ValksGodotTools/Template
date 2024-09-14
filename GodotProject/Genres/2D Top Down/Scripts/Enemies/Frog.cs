using CSharpUtils;
using Godot;
using GodotUtils;

using Template.TopDown2D;

namespace Template;

public partial class Frog : RigidBody
{
    [Export] FrogResource _config;
    [Export] AnimatedSprite2D _animatedSprite;
    [Export] Area2D _area;

    Player _player;
    Vector2 _landTarget;

    public override void _Ready()
    {
        base._Ready();

        Modulate = _config.Color;

        _area.SetDeferred(Area2D.PropertyName.Monitoring, false);
        _area.BodyEntered += body =>
        {
            if (EntityComponent.IsState("idle"))
            {
                if (body is Player player)
                {
                    this._player = player;
                    EntityComponent.SwitchState(PreJump());
                }
            }
        };
    }

    public override void IdleState(State state)
    {
        state.Enter = () =>
        {
            _animatedSprite.PlayRandom("idle");

            GTween.Delay(this, 1, () =>
            {
                _area.SetDeferred(Area2D.PropertyName.Monitoring, true);
            });
        };

        state.Exit = () =>
        {
            _area.SetDeferred(Area2D.PropertyName.Monitoring, false);
        };
    }

    State PreJump()
    {
        GTween tweenShake = new(_animatedSprite);
        Vector2 prevSpritePos = Vector2.Zero;

        State state = new(nameof(PreJump))
        {
            Enter = () =>
            {
                prevSpritePos = _animatedSprite.Position;
                _animatedSprite.Play("pre_jump");

                Vector2 shake_offset = new(2, 0.5f);
                double shake_duration = 0.05;

                tweenShake.SetAnimatingProp(Node2D.PropertyName.Position)
                    .AnimateProp(_animatedSprite.Position - shake_offset, shake_duration)
                    .AnimateProp(_animatedSprite.Position + shake_offset, shake_duration)
                    .Loop();

                GTween.Delay(this, 1, () =>
                {
                    EntityComponent.SwitchState(Jump());
                });
            },

            Exit = () =>
            {
                _animatedSprite.Position = prevSpritePos;
                _landTarget = _player.Position;
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
                _animatedSprite.Play("jump");

                this.SetCollisionLayerAndMask(3);

                Vector2 force = (_player.Position - Position) * _config.JumpForceInfluence;

                ApplyCentralImpulse(force);

                double jump_time = 1.0;

                new GTween(_animatedSprite)
                    .SetAnimatingProp(Node2D.PropertyName.Scale)
                    .AnimateProp(_animatedSprite.Scale * new Vector2(4, 2), 0.2).EaseOut()
                    .AnimateProp(_animatedSprite.Scale * new Vector2(2, 4), jump_time * 0.5).EaseOut()
                    .AnimateProp(_animatedSprite.Scale * new Vector2(1.5f, 0.5f), jump_time * 0.5).EaseIn()
                    .Callback(() =>
                    {
                        LinearVelocity = Vector2.Zero;
                        _animatedSprite.Play("idle");
                        this.SetCollisionLayerAndMask(2);
                    })
                    .AnimateProp(_animatedSprite.Scale * Vector2.One, jump_time * 0.5)
                    .Callback(() => EntityComponent.SwitchState(Idle()));
            }
        };

        return state;
    }
}

