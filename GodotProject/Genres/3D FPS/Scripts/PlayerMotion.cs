using Godot;

namespace Template.FPS3D;

public partial class Player : CharacterBody3D
{
    private float gravityForce = 10;
    private float jumpForce = 150;
    private float moveSpeed = 10;
    private float moveDampening = 20; // the higher the value, the less the player will slide

    private Vector3 gravityVec;

    private void OnPhysicsProcessMotion(double d)
    {
        MoveAndSlide();

        float delta = (float)d;
        float h_rot = _camera.Basis.GetEuler().Y;

        float f_input = -Input.GetAxis(InputActions.MoveDown, InputActions.MoveUp);
        float h_input = Input.GetAxis(InputActions.MoveLeft, InputActions.MoveRight);

        Vector3 dir = new Vector3(h_input, 0, f_input)
            .Rotated(Vector3.Up, h_rot) // Always face correct direction
            .Normalized(); // Prevent fast strafing movement

        if (IsOnFloor())
        {
            gravityVec = Vector3.Zero;

            if (Input.IsActionJustPressed(InputActions.Jump))
            {
                gravityVec = Vector3.Up * jumpForce * delta;
            }
        }
        else
        {
            gravityVec += Vector3.Down * gravityForce * delta;
        }

        Velocity = Velocity.Lerp(dir * moveSpeed, moveDampening * delta);
        Velocity += gravityVec;
    }
}

