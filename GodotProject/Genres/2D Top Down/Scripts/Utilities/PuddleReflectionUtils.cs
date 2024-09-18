using Godot;
using GodotUtils;

namespace Template.TopDown2D;

public static class PuddleReflectionUtils
{
    public static void CreateReflection(Node entity, Node2D sprite)
    {
        // Create reflection
        Node2D reflection = (Node2D)sprite.Duplicate();
        reflection.Name = "Reflection";

        if (reflection is Sprite2D sprite2D)
        {
            reflection.Position = new Vector2(0, sprite2D.GetPixelHeight());
        }
        else if (reflection is AnimatedSprite2D reflectionAnimated)
        {
            AnimatedSprite2D originalAnimatedSprite = sprite as AnimatedSprite2D;

            string[] animations = originalAnimatedSprite.SpriteFrames.GetAnimationNames();
            
            if (animations.Length > 0)
            {
                originalAnimatedSprite.Play(animations[0]);
            }

            reflection.Position = new Vector2(0, originalAnimatedSprite.GetPixelHeight());

            originalAnimatedSprite.AnimationChanged += () =>
            {
                reflectionAnimated.Play(originalAnimatedSprite.Animation);
            };
        }

        reflection.Modulate = Color.Color8(255, 255, 255, 36);
        reflection.Scale = new Vector2(reflection.Scale.X, -reflection.Scale.Y);
        reflection.LightMask = GMath.GetLayerValues(5);
        reflection.ZIndex = -9;
        reflection.Material = new CanvasItemMaterial
        {
            LightMode = CanvasItemMaterial.LightModeEnum.LightOnly
        };

        entity.AddChildDeferred(reflection);
    }
}

