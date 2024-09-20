using Godot;
using GodotUtils;
using System;
using System.Collections.Generic;

namespace Template;

public partial class ServiceProvider : Node
{
    public static ServiceProvider Services { get; private set; }
    
    private Dictionary<Type, Service> _services = [];

    public override void _EnterTree()
    {
        Services = this;
        Services.Add<Logger>();
    }

    /// <summary>
    /// Add a instance to be tracked as a service. If the service is marked
    /// as persistent it will not get cleaned up on scene change.
    /// </summary>
    public virtual Service Add(object instance, bool persistent = false)
    {
        Service service = new()
        {
            Instance = instance, 
            Persistent = persistent 
        };

        _services.Add(instance.GetType(), service);

        RemoveServiceOnSceneChanged(service);

        return service;
    }

    /// <summary>
    /// Add a object that does not exist within the game tree. For example
    /// the Logger class does not extend from Node.
    /// </summary>
    public Service Add<T>() where T : new()
    {
        T instance = new();

        Service service = new()
        { 
            Instance = instance, 
            Persistent = true // all instances that do not exist in the game tree
            // should be persistent
        };

        _services.Add(instance.GetType(), service);

        return service;
    }

    public T Get<T>()
    {
        if (!_services.ContainsKey(typeof(T)))
        {
            throw new Exception($"Unable to obtain service '{typeof(T)}'");
        }

        return (T)_services[typeof(T)].Instance;
    }

    public override string ToString()
    {
        return _services.ToFormattedString();
    }

    private void RemoveServiceOnSceneChanged(Service service)
    {
        // Do not remove persistent services
        // Only remove services if the SceneManager service exists in services
        if (service.Persistent || !_services.ContainsKey(typeof(SceneManager)))
        {
            return;
        }

        // The scene has changed, remove all non-persistent services
        SceneManager sceneManager = (SceneManager)_services[typeof(SceneManager)].Instance;
        sceneManager.PreSceneChanged += Cleanup;

        void Cleanup(string scene)
        {
            // Stop listening to PreSceneChanged
            sceneManager.PreSceneChanged -= Cleanup;

            // Remove the service
            bool success = _services.Remove(service.Instance.GetType());

            if (!success)
            {
                throw new Exception($"Failed to remove the service '{service.Instance.GetType().Name}'");
            }
        }
    }

    public class Service
    {
        public object Instance { get; set; }
        public bool Persistent { get; set; }
    }
}
