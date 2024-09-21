using Godot;

namespace Template.TopDown2D;

[GlobalClass, Icon("res://Template/Sprites/Icons/Gear/gear.svg")]
public partial class PlayerComponent : EntityComponent
{
    public override void TakeDamage(Vector2 direction = default)
    {
        base.TakeDamage();

        if (_entity is Player player)
        {
            player.ApplyExternalForce(direction * 200);
        }
    }
}
