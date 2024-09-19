using Godot;
using System;

namespace Template.TopDown2D;

public partial class OtherPlayer : Node2D
{
    public Vector2 LastServerPosition { get; set; }

    private float _smoothFactor;

    public override void _Ready()
    {
        // These values were all estimated manually, some values might be slightly inaccurate
        // If the heartbeat is set to 500 then the client will bounce towards next position regardless of what
        // value smooth factor is

        // The lower the smooth factor the smoother the transition
        // If the smooth factor is too low then the player will start to lag behind
        // If the smooth factor is too high then you will start to see glitchy movements because the
        // the position is constantly being clamped the last received server position
        _smoothFactor = Net.HeartbeatPosition switch
        {
            20 => 0.1f,
            50 => 0.075f,
            100 => 0.05f,
            200 => 0.02f,
            _ => throw new Exception("A smooth factor has not been defined for this heartbeat!"),
        };
    }

    public override void _PhysicsProcess(double delta)
    {
        float distance = Position.DistanceTo(LastServerPosition);

        // Move the current position towards the last known server position by a fraction of the distance.
        // The _smoothFactor determines how much of the distance to cover in this step. A _smoothFactor of
        // 1 would mean the position is instantly moved to the last known server position, while a
        // _smoothFactor of 0.5 would mean the position is moved halfway towards the last known server
        // position.
        Position = Position.MoveToward(LastServerPosition, distance * _smoothFactor);
    }

    public void SetLabelText(string text)
    {
        GetNode<Label>("Label").Text = text;
    }
}
