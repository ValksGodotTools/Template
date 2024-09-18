using Godot;
using GodotUtils;

namespace Template.TopDown2D;

[GlobalClass]
public partial class SlideState : NodeState
{
    protected override void Enter()
    {
        Vector2 target = Entity.Position + GMath.RandDir() * 100;
        Vector2 force = (target - Entity.Position) * 3;

        Entity.ApplyCentralImpulse(force);

        new GTween(Sprite)
            .SetAnimatingProp(Node2D.PropertyName.Scale)
            .AnimateProp(Sprite.Scale * new Vector2(1.3f, 0.5f), 0.2).EaseOut()
            .AnimateProp(Sprite.Scale * new Vector2(1, 1), 0.3).EaseOut()
            .Callback(() => SwitchState(NextState));
    }
}
