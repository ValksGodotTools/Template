using Template.Platformer2D;

namespace Template;

public class PlayerJumpState : PlayerState
{
    PlayerJumpVars jumpVars;

    public override void Enter()
    {
        jumpVars = Player.JumpVars;
        jumpVars.HoldingKey = true;
        jumpVars.LossBuildUp = 0;
        Player.Velocity -= new Vector2(0, jumpVars.Force);
    }

    public override void Update()
    {
        if (Input.IsActionPressed("jump") && jumpVars.HoldingKey)
        {
            jumpVars.LossBuildUp += jumpVars.Loss;
            Entity.Velocity -= new Vector2(
                x: 0, 
                y: Mathf.Max(0, jumpVars.Force - jumpVars.LossBuildUp));
        }

        if (Input.IsActionJustReleased("jump"))
        {
            jumpVars.HoldingKey = false;
        }
    }
}
