namespace GodotUtils;

using System;
using System.Timers;
using Object = System.Object;
using Timer = System.Timers.Timer;

/// <summary>
/// If for whatever reason a Timer is needed on a non-Godot thread, this is what you should use.
/// </summary>
public class STimer : IDisposable
{
    readonly Timer timer;

    public STimer(double delayMs, Action action, bool enabled = true, bool autoreset = true)
    {
        void Callback(Object source, ElapsedEventArgs e) => action();
        timer = new Timer(delayMs);
        timer.Enabled = enabled;
        timer.AutoReset = autoreset;
        timer.Elapsed += Callback;
    }

    public void Start() => timer.Enabled = true;
    public void Stop() => timer.Enabled = false;
    public void SetDelay(double delayMs) => timer.Interval = delayMs;

    public void Dispose() => timer.Dispose();
}
