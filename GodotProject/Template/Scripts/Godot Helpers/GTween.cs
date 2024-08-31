namespace GodotUtils;

using Godot;
using System;
using static Godot.Tween;

/// <summary>
/// The created GTween should be defined in _Ready() if it is going to be re-used
/// several times
/// </summary>
public class GTween
{
    PropertyTweener tweener;
    Tween tween;
    Node node;
    string animatingProperty;

    public GTween(Node node)
    {
        this.node = node;

        // Ensure the Tween is fresh when re-creating it
        Kill();
        tween = node.CreateTween();

        // This helps to prevent the camera from lagging behind the players movement
        tween.SetProcessMode(Tween.TweenProcessMode.Physics);
    }

    /// <summary>
    /// Creates a looping tween that will stop and execute a callback when a condition is met.
    /// </summary>
    public static GTween Loop(Node node, double duration, Func<bool> condition, Action callback)
    {
        GTween tween = new(node);
        tween.Delay(duration)
            .Loop()
            .Callback(() =>
            {
                if (condition())
                {
                    tween.Stop();
                    callback();
                }
            });

        return tween;
    }

    /// <summary>
    /// Creates a delay followed by a callback only executed after the delay
    /// </summary>
    public static GTween Delay(Node node, double duration, Action callback)
    {
        GTween tween = new(node);

        tween.Delay(duration)
            .Callback(callback);

        return tween;
    }

    /// <summary>
    /// Animates the property that was set with SetAnimatingProp(string prop)
    /// 
    /// <code>
    /// tween.SetAnimatingProp(ColorRect.PropertyName.Color);
    /// tween.AnimateProp(Colors.Transparent, 0.5);
    /// </code>
    /// </summary>
    public GTween AnimateProp(Variant finalValue, double duration)
    {
        if (string.IsNullOrWhiteSpace(animatingProperty))
            throw new Exception("No animation property has been set with tween.SetAnimatingProp(...)");

        return Animate(animatingProperty, finalValue, duration);
    }

    /// <summary>
    /// Animates a specified property. All tweens use the Sine transition by default.
    /// 
    /// <code>
    /// tween.Animate(ColorRect.PropertyName.Color, Colors.Transparent, 0.5);
    /// </code>
    /// </summary>
    public GTween Animate(string prop, Variant finalValue, double duration)
    {
        tweener = tween
            .TweenProperty(node, prop, finalValue, duration)
            .SetTrans(Tween.TransitionType.Sine);

        return this;
    }

    /// <summary>
    /// Sets the property to be animated on
    /// 
    /// <code>
    /// tween.SetAnimatingProp(ColorRect.PropertyName.Color);
    /// tween.AnimateProp(Colors.Transparent, 0.5);
    /// </code>
    /// </summary>
    public GTween SetAnimatingProp(string animatingProperty)
    {
        this.animatingProperty = animatingProperty;
        return this;
    }

    public GTween SetProcessMode(Tween.TweenProcessMode mode)
    {
        tween = tween.SetProcessMode(mode);
        return this;
    }

    /// <summary>
    /// Sets the animation to repeat
    /// </summary>
    public GTween Loop(int loops = 0)
    {
        tween = tween.SetLoops(loops);
        return this;
    }

    /// <summary>
    /// <para>Makes the next <see cref="Godot.Tweener"/> run parallelly to the previous one.</para>
    /// <para><b>Example:</b></para>
    /// <para><code>
    /// GTween tween = new(...);
    /// tween.Animate(...);
    /// tween.Parallel().Animate(...);
    /// tween.Parallel().Animate(...);
    /// </code></para>
    /// <para>All <see cref="Godot.Tweener"/>s in the example will run at the same time.</para>
    /// <para>You can make the <see cref="Godot.Tween"/> parallel by default by using <see cref="Godot.Tween.SetParallel(bool)"/>.</para>
    /// </summary>
    public GTween Parallel()
    {
        tween = tween.Parallel();
        return this;
    }

    /// <summary>
    /// <para>If <paramref name="parallel"/> is <see langword="true"/>, the <see cref="Godot.Tweener"/>s appended after this method will by default run simultaneously, as opposed to sequentially.</para>
    /// <para><code>
    /// tween.SetParallel()
    /// tween.Animate(...)
    /// tween.Animate(...)
    /// </code></para>
    /// </summary>
    public GTween SetParallel(bool parallel = true)
    {
        tween = tween.SetParallel(parallel);
        return this;
    }

    public GTween Callback(Action callback)
    {
        tween.TweenCallback(Callable.From(callback));
        return this;
    }

    public GTween Delay(double duration)
    {
        tween.TweenCallback(Callable.From(() => { /* Empty Action */ })).SetDelay(duration);
        return this;
    }

    /// <summary>
    /// Executed when the tween has finished
    /// </summary>
    public GTween Finished(Action callback)
    {
        tween.Finished += callback;
        return this;
    }

    /// <summary>
    /// If the tween is looping, this can be used to stop it
    /// </summary>
    public GTween Stop()
    {
        tween.Stop();
        return this;
    }

    /// <summary>
    /// Pause the tween
    /// </summary>
    public GTween Pause()
    {
        tween.Pause();
        return this;
    }

    /// <summary>
    /// If the tween was paused with Pause(), resume it with Resume()
    /// </summary>
    public GTween Resume()
    {
        tween.Play();
        return this;
    }

    /// <summary>
    /// Kill the tween
    /// </summary>
    public GTween Kill()
    {
        tween?.Kill();
        return this;
    }

    public GTween SetTrans(Tween.TransitionType transType)
    {
        return UpdateTweener(nameof(SetTrans), () => tweener.SetTrans(transType));
    }

    public GTween SetEase(Tween.EaseType easeType)
    {
        return UpdateTweener(nameof(SetEase), () => tweener.SetEase(easeType));
    }

    public GTween TransLinear()
    {
        return UpdateTweener(nameof(TransLinear), () => tweener.SetTrans(TransitionType.Linear));
    }

    public GTween TransBack()
    {
        return UpdateTweener(nameof(TransBack), () => tweener.SetTrans(TransitionType.Back));
    }

    public GTween TransSine()
    {
        return UpdateTweener(nameof(TransSine), () => tweener.SetTrans(TransitionType.Sine));
    }

    public GTween TransBounce()
    {
        return UpdateTweener(nameof(TransBounce), () => tweener.SetTrans(TransitionType.Bounce));
    }

    public GTween TransCirc()
    {
        return UpdateTweener(nameof(TransCirc), () => tweener.SetTrans(TransitionType.Circ));
    }

    public GTween TransCubic()
    {
        return UpdateTweener(nameof(TransCubic), () => tweener.SetTrans(TransitionType.Cubic));
    }

    public GTween TransElastic()
    {
        return UpdateTweener(nameof(TransElastic), () => tweener.SetTrans(TransitionType.Elastic));
    }

    public GTween TransExpo()
    {
        return UpdateTweener(nameof(TransExpo), () => tweener.SetTrans(TransitionType.Expo));
    }

    public GTween TransQuad()
    {
        return UpdateTweener(nameof(TransQuad), () => tweener.SetTrans(TransitionType.Quad));
    }

    public GTween TransQuart()
    {
        return UpdateTweener(nameof(TransQuart), () => tweener.SetTrans(TransitionType.Quart));
    }

    public GTween TransQuint()
    {
        return UpdateTweener(nameof(TransQuint), () => tweener.SetTrans(TransitionType.Quint));
    }

    public GTween TransSpring()
    {
        return UpdateTweener(nameof(TransSpring), () => tweener.SetTrans(TransitionType.Spring));
    }

    public GTween EaseIn()
    {
        return UpdateTweener(nameof(EaseIn), () => tweener.SetEase(EaseType.In));
    }

    public GTween EaseOut()
    {
        return UpdateTweener(nameof(EaseOut), () => tweener.SetEase(EaseType.Out));
    }

    public GTween EaseInOut()
    {
        return UpdateTweener(nameof(EaseInOut), () => tweener.SetEase(EaseType.InOut));
    }

    public GTween EaseOutIn()
    {
        return UpdateTweener(nameof(EaseOutIn), () => tweener.SetEase(EaseType.OutIn));
    }

    /// <summary>
    /// Checks if the tween is still playing
    /// </summary>
    public bool IsRunning() => tween.IsRunning();

    private GTween UpdateTweener(string methodName, Action action)
    {
        if (tweener == null)
            throw new Exception($"Cannot call {methodName}() because no tweener has been set with tween.Animate(...)");

        action();
        return this;
    }
}
