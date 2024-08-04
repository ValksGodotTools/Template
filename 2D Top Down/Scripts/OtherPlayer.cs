namespace Template;

using GodotUtils.Netcode;

public partial class OtherPlayer : Node2D
{
    public PrevCurQueue<Vector2> PrevCurPos { get; } = new(GameServer.HeartbeatPosition);

    public void SetLabelText(string text) => GetNode<Label>("Label").Text = text;

    public override void _PhysicsProcess(double delta)
    {
        PrevCurPos.UpdateProgress(delta);

        Position = PrevCurPos.Previous.Lerp(PrevCurPos.Current, PrevCurPos.Progress);
    }
}
