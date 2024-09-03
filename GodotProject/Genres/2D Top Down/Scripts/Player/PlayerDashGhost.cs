namespace Template;

public partial class PlayerDashGhost : Node2D
{
    public override void _Ready()
    {
        const double MODULATE_DURATION = 0.5;

        // Animate all canvas items modulate (anything attached to root)
        new GTween(this)
            .Animate(Node2D.PropertyName.Modulate, Colors.Transparent, MODULATE_DURATION).EaseOut();
    }
}
