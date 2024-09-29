using System.Collections.Generic;
using System.Linq;
using System;

namespace GodotUtils;

/// <summary>
/// This class was created to attempt to simplify the process of creating C# events for gamedev.
/// 
/// ########### Example #1 ###########
/// 
/// Events.Generic.AddListener(EventGeneric.OnKeyboardInput, (args) => 
/// {
///     GD.Print(args[0]);
///     GD.Print(args[1]);
///     GD.Print(args[2]);
/// }, "someId");
/// 
/// Events.Generic.RemoveListeners(EventGeneric.OnKeyboardInput, "someId");
/// 
/// // Listener is never called because it was removed
/// Events.Generic.Notify(EventGeneric.OnKeyboardInput, 1, 2, 3);
/// 
/// ########### Example #2 ###########
/// Events.Player.AddListener<PlayerSpawnArgs>(EventPlayer.OnPlayerSpawn, (args) => 
/// {
///     GD.Print(args.Name);
///     GD.Print(args.Location);
///     GD.Print(args.Player);
/// });
/// 
/// Events.Player.Notify(EventPlayer.OnPlayerSpawn, new PlayerSpawnArgs(name, location, player));
/// </summary>
/// <typeparam name="TEvent">The event type enum to be used. For example 'EventPlayer' enum.</typeparam>
public class EventManager<TEvent>
{
    private readonly Dictionary<TEvent, List<object>> eventListeners = [];

    /// <summary>
    /// The event type to be listened to (Action uses object[] params by default)
    /// </summary>
    public void AddListener(TEvent eventType, Action<object[]> action, string id = "")
    {
        AddListener<object[]>(eventType, action, id);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1854:Prefer the 'IDictionary.TryGetValue(TKey, out TValue)' method", Justification = "Don't have time to test this right now")]
    public void AddListener<T>(TEvent eventType, Action<T> action, string id = "")
    {
        if (!eventListeners.ContainsKey(eventType))
        {
            eventListeners.Add(eventType, []);
        }

        eventListeners[eventType].Add(new Listener(action, id));
    }

    /// <summary>
    /// Remove all listeners of type 'eventType' with 'id'
    /// For example. If there is a listener of type OnPlayerSpawn with id 1 and another
    /// with id 1 (same id). Then this function will remove both these listeners.
    /// </summary>
    public void RemoveListeners(TEvent eventType, string id = "")
    {
        if (!eventListeners.ContainsKey(eventType))
        {
            throw new InvalidOperationException($"Tried to remove listener of event type '{eventType}' from an event type that has not even been defined yet");
        }

        foreach (KeyValuePair<TEvent, List<object>> pair in eventListeners)
        {
            for (int i = pair.Value.Count - 1; i >= 0; i--)
            {
                if (pair.Key.Equals(eventType) && ((Listener)pair.Value[i]).Id == id)
                {
                    pair.Value.RemoveAt(i);
                }
            }
        }
    }

    /// <summary>
    /// Remove ALL listeners from ALL event types
    /// </summary>
    public void RemoveAllListeners()
    {
        eventListeners.Clear();
    }

    /// <summary>
    /// Notify all listeners
    /// </summary>
    public void Notify(TEvent eventType, params object[] args)
    {
        if (!eventListeners.TryGetValue(eventType, out List<object> value))
        {
            return;
        }

        foreach (dynamic listener in value.ToList()) // if ToList() is not here then issue #137 will occur
        {
            listener.Action(args);
        }
    }
}

public class Listener(dynamic action, string id)
{
    public dynamic Action { get; set; } = action;
    public string Id { get; set; } = id;
}
