namespace GodotUtils;

using Godot;
using System;

public class GTimer
{
    public event Action Timeout;

    private Timer timer;

    public GTimer(Node node, double milliseconds, bool looping)
    {
        timer = new Timer();
        timer.ProcessCallback = Timer.TimerProcessCallback.Physics;
        timer.OneShot = !looping;
        timer.WaitTime = milliseconds * 0.001; // Convert from milliseconds to seconds
        node.AddChild(timer);
        timer.Timeout += () => Timeout?.Invoke();
    }

    public void Start() => timer.Start();
    public void Stop() => timer.Stop();
}
