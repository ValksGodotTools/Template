namespace Template.Platformer2D;

public partial class Player : Entity
{
    PlayerJumpVars jumpVars { get; } = new();

    State jump;

    void StateJump()
    {
        jump = new(this, "Jump");


        jump.Enter = () =>
        {
            jumpVars.HoldingKey = true;
            jumpVars.LossBuildUp = 0;
            Velocity -= new Vector2(0, jumpVars.Force);
        };


        jump.Update = () =>
        {
            if (Input.IsActionPressed("jump") && jumpVars.HoldingKey)
            {
                jumpVars.LossBuildUp += jumpVars.Loss;
                Velocity -= new Vector2(
                    x: 0,
                    y: Mathf.Max(0, jumpVars.Force - jumpVars.LossBuildUp));
            }

            if (Input.IsActionJustReleased("jump"))
            {
                jumpVars.HoldingKey = false;
            }
        };


        jump.Transitions = () =>
        {
            if (IsOnFloor())
                SwitchState(idle);
        };
    }

    public class PlayerJumpVars
    {
        public float Force { get; } = 100;
        public float Loss { get; } = 7.5f;
        public float LossBuildUp { get; set; }
        public bool HoldingKey { get; set; } // the jump key
    }
}
