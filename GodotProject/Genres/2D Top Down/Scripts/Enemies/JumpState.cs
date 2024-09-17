﻿using CSharpUtils;
using Godot;
using GodotUtils;
using Template.TopDown2D;

namespace Template;

[GlobalClass]
public partial class JumpState : NodeState
{
    [Export] private AnimatedSprite2D _animatedSprite;
    [Export] private NodeState _idleState;
    [Export] private Area2D _playerDetectArea;

    public override State GetState()
    {
        GTween tweenShake = new(_animatedSprite);
        Vector2 prevSpritePos = Vector2.Zero;

        State state = new("Pre Jump")
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
                    SwitchState(Jump());
                });
            },

            Exit = () =>
            {
                _animatedSprite.Position = prevSpritePos;
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

                Entity.SetCollisionLayerAndMask(3);

                Vector2 force = (Player.Position - Entity.Position) * 2;

                Entity.ApplyCentralImpulse(force);

                double jump_time = 1.0;

                new GTween(_animatedSprite)
                    .SetAnimatingProp(Node2D.PropertyName.Scale)
                    .AnimateProp(_animatedSprite.Scale * new Vector2(4, 2), 0.2).EaseOut()
                    .AnimateProp(_animatedSprite.Scale * new Vector2(2, 4), jump_time * 0.5).EaseOut()
                    .AnimateProp(_animatedSprite.Scale * new Vector2(1.5f, 0.5f), jump_time * 0.5).EaseIn()
                    .Callback(() =>
                    {
                        Entity.LinearVelocity = Vector2.Zero;
                        _animatedSprite.Play("idle");
                        Entity.SetCollisionLayerAndMask(2);
                    })
                    .AnimateProp(_animatedSprite.Scale * Vector2.One, jump_time * 0.5)
                    .Callback(() => SwitchState(_idleState));
            }
        };

        return state;
    }
}
