namespace GodotUtils;

using Godot;

public static class ExtensionsAnimationTree
{
    /// <summary>
    /// Set a condition to 'value' then flip 'value' when 0.1 seconds have passed.
    /// Conditions only need to be set for a short duration otherwise you will find
    /// yourself trying to set all the conditions you set earlier to false again.
    /// 
    /// E.g. SetCondition("reload", true)
    /// </summary>
    public static void SetCondition(this AnimationTree tree, StringName path, bool value)
    {
        tree.SetParam($"conditions/{path}", value);

        new GTween(tree)
            .Delay(0.1)
            .Callback(() => tree.SetParam($"conditions/{path}", !value));
    }

    /// <summary>
    /// The name is the name of the BlendSpace1D in the AnimationTree inspector. By default this
    /// is called "BlendSpace1D".
    /// </summary>
    public static void SetBlendSpace1DPosition(this AnimationTree tree, StringName name, float value) =>
        tree.SetParam($"{name}/blend_position", value);

    public static void SetParam(this AnimationTree tree, StringName path, Variant value) =>
        tree.Set($"parameters/{path}", value);

    public static Variant GetParam(this AnimationTree tree, StringName path) =>
        tree.Get($"parameters/{path}");

    public static bool GetCondition(this AnimationTree tree, StringName path) =>
        (bool)tree.GetParam($"conditions/{path}");

    public static AnimationNodeStateMachinePlayback GetStateMachine(this AnimationTree tree) =>
        tree.Get("parameters/playback").As<AnimationNodeStateMachinePlayback>();
}
