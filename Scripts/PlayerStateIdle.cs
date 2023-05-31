using Template.Platformer2D;

namespace Template;

public class PlayerStateIdle : PlayerState
{
    public override void Enter()
    {
        Entity.Sprite.Play("idle");
    }

    public override void Update()
    {
        if (Input.IsActionJustPressed("jump") && Player.IsOnFloor())
        {
            Switch(new PlayerJumpState { Player = Player });
        }
    }
}
