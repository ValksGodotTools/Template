using CSharpUtils;
using Godot;
using GodotUtils;
using Template.TopDown2D;

namespace Template;

public partial class Frog : RigidBody2D
{
    [Export] private FrogResource _config;
    [Export] private AnimatedSprite2D _animatedSprite;
    [Export] private Area2D _area;

    private Player _player;
    private Vector2 _landTarget;
    private EntityComponent _entityComponent;

    public override void _Ready()
    {
        _entityComponent = this.GetNode<EntityComponent>();
        _entityComponent.SwitchState(Idle());

        Modulate = _config.Color;

        _area.SetDeferred(Area2D.PropertyName.Monitoring, false);
        _area.BodyEntered += body =>
        {
            if (_entityComponent.IsState("idle"))
            {
                if (body is Player player)
                {
                    _player = player;
                    _entityComponent.SwitchState(PreJump());
                }
            }
        };
    }

    public State Idle()
    {
        State state = new(nameof(Idle))
        {
            Enter = () =>
            {
                _animatedSprite.PlayRandom("idle");

                GTween.Delay(this, 1, () =>
                {
                    _area.SetDeferred(Area2D.PropertyName.Monitoring, true);
                });
            },
            Exit = () =>
            {
                _area.SetDeferred(Area2D.PropertyName.Monitoring, false);
            }
        };

        return state;
    }

    private State Slide()
    {
        State state = new(nameof(Slide))
        {
            Enter = () =>
            {
                Vector2 target = Position + GMath.RandDir() * 100;
                Vector2 force = (target - Position);

                ApplyCentralImpulse(force);

                new GTween(_animatedSprite)
                    .SetAnimatingProp(Node2D.PropertyName.Scale)
                    .AnimateProp(_animatedSprite.Scale * new Vector2(1.3f, 0.5f), 0.2).EaseOut()
                    .AnimateProp(_animatedSprite.Scale * new Vector2(1, 1), 0.3).EaseOut()
                    .Callback(() => _entityComponent.SwitchState(Idle()));
            }
        };

        return state;
    }

    private State PreJump()
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
                    _entityComponent.SwitchState(Jump());
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

    private State Jump()
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
                    .Callback(() => _entityComponent.SwitchState(Idle()));
            }
        };

        return state;
    }
}

