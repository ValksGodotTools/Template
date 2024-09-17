using Godot;
using GodotUtils;

namespace Template.TopDown2D;

public static class PuddleReflectionUtils
{
    public static void CreateReflection(Node entity)
    {
        Node2D sprite = FindSprite(entity);

        if (sprite == null)
            return;

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
            originalAnimatedSprite.Play("idle");

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

    private static Node2D FindSprite(Node entity)
    {
        Sprite2D sprite = entity.GetNode<Sprite2D>();

        if (sprite != null)
            return sprite;

        AnimatedSprite2D spriteAnimated = entity.GetNode<AnimatedSprite2D>();

        return spriteAnimated ?? null;
    }
}

