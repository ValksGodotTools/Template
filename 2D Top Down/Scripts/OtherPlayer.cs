namespace Template;

using Template.Netcode;

public partial class OtherPlayer : Node2D
{
    public PrevCurNetQueue<Vector2> PrevCurPos { get; } = new(Net.HeartbeatPosition);

    public void SetLabelText(string text) => GetNode<Label>("Label").Text = text;

    public override void _PhysicsProcess(double delta)
    {
        PrevCurPos.UpdateProgress(delta);

        Position = PrevCurPos.Previous.Lerp(PrevCurPos.Current, PrevCurPos.Progress);
    }
}
