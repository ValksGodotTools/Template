using Godot;

namespace Template.TopDown2D;

[GlobalClass, Icon("res://Template/Sprites/Icons/Gear/gear.svg")]
public partial class EntityComponent : Node2D
{
    [Export] private EntityConfig _config;
    [Export] public AnimatedSprite2D AnimatedSprite { get; private set; }
    
    private Node2D _entity;

    public override void _Ready()
    {
        _entity = GetOwner<Node2D>();

        SetMaterial();
        CreateReflection();
    }

    private void SetMaterial()
    {
        AnimatedSprite.SelfModulate = _config.Color;

        AnimatedSprite.Material ??= new CanvasItemMaterial()
        {
            BlendMode = _config.BlendMode,
            LightMode = _config.LightMode
        };
    }

    private void CreateReflection()
    {
        PuddleReflectionUtils.CreateReflection(_entity, AnimatedSprite);
    }
}
