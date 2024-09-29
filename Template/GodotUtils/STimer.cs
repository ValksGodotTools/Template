using Object = System.Object;
using System.Timers;
using System;
using Timer = System.Timers.Timer;

namespace GodotUtils;

/// <summary>
/// If for whatever reason a Timer is needed on a non-Godot thread, this is what you should use.
/// </summary>
public class STimer : IDisposable
{
    private readonly Timer _timer;

    public STimer(double delayMs, Action action, bool enabled = true, bool autoreset = true)
    {
        void Callback(Object source, ElapsedEventArgs e) => action();
        _timer = new Timer(delayMs);
        _timer.Enabled = enabled;
        _timer.AutoReset = autoreset;
        _timer.Elapsed += Callback;
    }

    public void Start()
    {
        _timer.Enabled = true;
    }

    public void Stop()
    {
        _timer.Enabled = false;
    }

    public void SetDelay(double delayMs)
    {
        _timer.Interval = delayMs;
    }

    public void Dispose()
    {
        _timer.Dispose();
    }
}
