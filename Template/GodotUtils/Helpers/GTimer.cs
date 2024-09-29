using Godot;
using System;
using Timer = Godot.Timer;

namespace GodotUtils;

public class GTimer
{
    public event Action Timeout;

    private Timer _timer;

    public GTimer(Node node, double milliseconds, bool looping)
    {
        _timer = new Timer();
        _timer.ProcessCallback = Timer.TimerProcessCallback.Physics;
        _timer.OneShot = !looping;
        _timer.WaitTime = milliseconds * 0.001; // Convert from milliseconds to seconds
        node.AddChild(_timer);
        _timer.Timeout += () => Timeout?.Invoke();
    }

    public void Start()
    {
        _timer.Start();
    }

    public void Stop()
    {
        _timer.Stop();
    }
}

