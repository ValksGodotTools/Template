using Godot;
using GodotUtils;

namespace Template.TopDown2D;

[GlobalClass, Icon("res://Template/Sprites/Icons/Gear/gear.svg")]
public partial class EntityComponent : Node2D
{
    [Export] private EntityConfig _config;
    [Export] public AnimatedSprite2D AnimatedSprite { get; private set; }
    
    private Node2D _entity;
    private ShaderMaterial _shaderMaterial;

    public override void _Ready()
    {
        _entity = GetOwner<Node2D>();
        _shaderMaterial = AnimatedSprite.Material as ShaderMaterial;

        SetMaterial();
        CreateReflection();
    }

    public void TakeDamage()
    {
        new GTween(this)
            .SetAnimatingShaderMaterial(_shaderMaterial)
            .AnimateShader("blend_intensity", 1.0f, 0.1f)
            .AnimateShader("blend_intensity", 0.0f, 0.3f).EaseOut();
    }

    private void SetMaterial()
    {
        AnimatedSprite.SelfModulate = _config.Color;

        /*AnimatedSprite.Material ??= new CanvasItemMaterial()
        {
            BlendMode = _config.BlendMode,
            LightMode = _config.LightMode
        };*/
    }

    private void CreateReflection()
    {
        PuddleReflectionUtils.CreateReflection(_entity, AnimatedSprite);
    }
}
