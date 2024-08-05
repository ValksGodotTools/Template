namespace Template.TopDown2D;

using GodotUtils.World2D.TopDown;

public partial class Player : CharacterBody2D
{
    #region Config

    float speed = 50;
    float friction = 0.1f;

    #endregion

    #region Variables

    GTimer positionEmitInterval;
    Vector2 prevPosition;

    #endregion

    public override void _PhysicsProcess(double delta)
    {
        MoveAndSlide();

        // Velocity is mutiplied by delta for us already
        Velocity += Utils.GetMovementInput() * speed;
        Velocity = Velocity.Lerp(Vector2.Zero, friction);
    }

    public void StartNet()
    {
        Net net = Global.Services.Get<Net>();

        positionEmitInterval = new(this,
            milliseconds: Net.HeartbeatPosition, looping: true);

        positionEmitInterval.Timeout += () =>
        {
            if (Position != prevPosition)
            {
                net.Client.Send(new CPacketPosition
                {
                    Position = Position
                });
            } 

            prevPosition = Position;
        };
        positionEmitInterval.Start();
    }
}
