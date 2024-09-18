using Godot;
using GodotUtils;

namespace Template.TopDown2D;

[GlobalClass]
public partial class ShakeState : NodeState
{
    protected override void Enter()
    {
        GTween tweenShake = new(Sprite);
        Vector2 prevSpritePos = Sprite.Position;

        Sprite.Play("pre_jump");

        Vector2 shake_offset = new(2, 0.5f);
        double shake_duration = 0.05;

        tweenShake.SetAnimatingProp(Node2D.PropertyName.Position)
            .AnimateProp(Sprite.Position - shake_offset, shake_duration)
            .AnimateProp(Sprite.Position + shake_offset, shake_duration)
            .Loop();

        GTween.Delay(this, 1, () =>
        {
            tweenShake.Stop();
            Sprite.Position = prevSpritePos;
            SwitchState(NextState);
        });
    }
}
