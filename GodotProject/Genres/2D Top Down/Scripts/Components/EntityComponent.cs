using Godot;
using GodotUtils;

namespace Template.TopDown2D;

[GlobalClass, Icon("res://Template/Sprites/Icons/Gear/gear.svg")]
public partial class EntityComponent : Node2D
{
    [Export] private EntityConfig _config;
    [Export] public AnimatedSprite2D AnimatedSprite { get; private set; }
    
    private Node2D _entity;
    private GTween _flashWhite;
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
        Tween tween = CreateTween();

        string shaderParam = "shader_parameter/blend_intensity";

        tween.TweenProperty(_shaderMaterial, shaderParam, 1.0f, 0.1f)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.In);

        tween.TweenProperty(_shaderMaterial, shaderParam, 0.0f, 0.3f)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.Out);
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
