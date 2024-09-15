using Godot;

namespace Template;

public partial class PlayerDashGhost : Node2D
{
    public override void _Ready()
    {
        Name = nameof(PlayerDashGhost);

        const double MODULATE_DURATION = 0.5;

        new GTween(this)
            .Animate(Node2D.PropertyName.Modulate, Colors.Transparent, MODULATE_DURATION).EaseOut()
            .Callback(QueueFree);
    }
}

