namespace Template;

public partial class PlayerDashGhost : Node2D
{
    public override void _Ready()
    {
        const double MODULATE_DURATION = 0.5;
        const double LIGHT_DURATION = 3.0;

        // Animate all canvas items modulate (anything attached to root)
        new GTween(this)
            .Animate(Node2D.PropertyName.Modulate, Colors.Transparent, MODULATE_DURATION).EaseOut();

        // Animate light
        new GTween(GetNode<PointLight2D>("PointLight2D"))
            .Animate(PointLight2D.PropertyName.Energy, 0, LIGHT_DURATION).EaseOut()
            .Callback(() => QueueFree());
    }
}
