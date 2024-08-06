namespace Template.Netcode;

using ENet;
using Godot;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

public class PrevCurQueue<T>
{
    public float Progress { get; set; }

    public T Previous { get; set; }
    public T Current { get; set; }

    /// <summary>
    /// Should this keep updating even after a Progress of 1.0 has been reached?
    /// If this is set to true then if say a position packet is not received in
    /// time, then the player will continue moving in the last known direction.
    /// If this is set to false then the player will just come to a direct halt
    /// when a Progress value of 1.0 is reached.
    /// </summary>
    public bool KeepUpdating { get; set; }

    readonly List<T> data = new();
    int interval;

    public PrevCurQueue(int interval)
    {
        this.interval = interval;
        Current = default(T);
    }

    public void Add(T newData)
    {
        Progress = 0; // reset progress as this is new incoming data

        if (data.Count == 0)
        {
            Previous = newData;
            Current = newData;
        }

        data.Add(newData);

        if (data.Count > 2) // only keep track of previous and current
            data.RemoveAt(0);

        if (data.Count == 1)
        {
            Previous = data[0];
        }

        if (data.Count == 2)
        {
            Previous = data[0];
            Current = data[1];
        }
    }

    /// <summary>
    /// <para>There are 60 Frames in 1 Second (60 FPS)</para>
    /// 
    /// <para>
    /// If we set Interval to 1000 then UpdateProgress will have to be
    /// called 60 times for Progress to reach a value of 1.0
    /// That is it will happen in 1 second
    /// </para>
    /// 
    /// <para>
    /// If we set Interval to 2000 then UpdateProgress will have to be
    /// called 120 times for Progress to reach a value of 1.0
    /// That is it will happen in 2 seconds
    /// </para>
    /// 
    /// <para>
    /// Interval is the update interval. So if the server and client are
    /// both sending say position updates every 50ms, then the Interval
    /// should be set to 50ms
    /// </para>
    /// </summary>
    public void UpdateProgress(double delta)
    {
        if (Progress < 1.0)
            AddToProgress(delta);
        else
        {
            if (KeepUpdating)
                AddToProgress(delta);
        }
    }

    void AddToProgress(double delta) => Progress += (float)delta * (1000f / interval);
}
