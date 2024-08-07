namespace Template;

using Template.Netcode;

public partial class OtherPlayer : Node2D
{
    public Vector2 LastServerPosition { get; set; }

    float smoothFactor;

    public override void _Ready()
    {
        // These values were all estimated manually, some values might be slightly inaccurate
        // If the heartbeat is set to 500 then the client will bounce towards next position regardless of what
        // value smooth factor is

        // The lower the smooth factor the smoother the transition
        // If the smooth factor is too low then the player will start to lag behind
        // If the smooth factor is too high then you will start to see glitchy movements because the
        // the position is constantly being clamped the last received server position
        switch (Net.HeartbeatPosition)
        {
            case 20:
                smoothFactor = 0.1f;
                break;
            case 50:
                smoothFactor = 0.075f;
                break;
            case 100:
                smoothFactor = 0.05f;
                break;
            case 200:
                smoothFactor = 0.02f;
                break;
            default:
                throw new Exception("A smooth factor has not been defined for this heartbeat!");
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        float distance = Position.DistanceTo(LastServerPosition);
        int distanceCompensationFactor = 5;

        // If the distance to the last received server position is close enough then move towards it at a speed
        // multiplied by a factor of the remaining distance. If the distance is too far away then instantly
        // teleport to the last received server position.
        Position = distance <= Net.HeartbeatPosition * distanceCompensationFactor ?
            Position.MoveToward(LastServerPosition, distance * smoothFactor) : LastServerPosition;
    }

    public void SetLabelText(string text) => GetNode<Label>("Label").Text = text;
}
