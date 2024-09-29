using Godot;
using System;

namespace GodotUtils;

public partial class XTimer : Node
{
    public event Action Timeout;

    /// <summary>
    /// The current time passed in seconds
    /// </summary>
    public double Time { get; set; }

    /// <summary>
    /// The delay in seconds the timer 'Time' needs to reach. If 'Time' reaches
    /// 'Delay', then the 'Timeout' delegate is invoked.
    /// </summary>
    public double Delay { get; set; }

    private Action<double> _processCallback;

    /// <summary>
    /// Creates a new timer with a set delay in seconds
    /// </summary>
    /// <param name="parent">The parent node to attach this timer to</param>
    /// <param name="delay">The delay in seconds</param>
    /// <param name="repeating">Should this timer repeat?</param>
    public XTimer(Node parent, double delay, bool repeating = false)
    {
        Delay = delay;

        parent.AddChild(this);
        SetPhysicsProcess(false);

        if (repeating)
        {
            _processCallback = delta =>
            {
                Time += delta;

                if (Time >= Delay)
                {
                    Time = 0;
                    Timeout?.Invoke();
                }
            };
        }
        else
        {
            _processCallback = delta =>
            {
                Time += delta;

                if (Time >= Delay)
                {
                    Stop();
                    Timeout?.Invoke();
                }
            };
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        _processCallback(delta);
    }

    /// <summary>
    /// Set the time to delay. Useful if for example guns should be pre-loaded
    /// on game start.
    /// </summary>
    public void MaximizeTime()
    {
        Time = Delay;
    }

    /// <summary>
    /// Start the timer
    /// </summary>
    public void Start()
    {
        SetPhysicsProcess(true);
    }

    /// <summary>
    /// Stop the timer
    /// </summary>
    public void Stop()
    {
        SetPhysicsProcess(false);
    }

    /// <summary>
    /// Reset the timer by setting the time to 0 and starting the timer again
    /// </summary>
    public void Reset()
    {
        Time = 0;
        Start();
    }

    /// <summary>
    /// Is Time still less than Delay?
    /// </summary>
    public bool IsActive()
    {
        return Time < Delay;
    }
}

