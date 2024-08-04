namespace Template;

public partial class OtherPlayer : Node2D
{
    public void SetLabelText(string text) => GetNode<Label>("Label").Text = text;
}
