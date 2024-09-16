using CSharpUtils;
using Godot;

namespace Template.Platformer2D.Retro;

public partial class Player : Entity
{
    float _maxSpeed = 500;
    float _acceleration = 40;
    float _friction = 20;
    float _gravity = 20;

    public override void Update()
    {
        Vector2 vel = Velocity;
        float horzDir = Input.GetAxis("move_left", "move_right");

        // Horizontal movement
        vel.X += horzDir * _acceleration;
        vel.X = Utils.ClampAndDampen(vel.X, _friction, _maxSpeed);

        // Gravity
        vel.Y += _gravity;

        Velocity = vel;
    }

    protected override State InitialState()
    {
        return Idle();
    }
}

