using Godot;
using RedotUtils;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Template;

/// <summary>
/// A service provider class that manages the registration and retrieval of services in a Godot project.
/// </summary>
public partial class Services : Node
{
    /// <summary>
    /// Dictionary to store registered services, keyed by their type.
    /// </summary>
    private static Dictionary<Type, Service> _services = [];

    public override void _EnterTree()
    {
        GetTree().NodeAdded += RegisterService;
    }

    /// <summary>
    /// Retrieves a service of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the service to retrieve.</typeparam>
    /// <returns>The instance of the service.</returns>
    /// <exception cref="Exception">Thrown if the service is not found.</exception>
    public static T Get<T>()
    {
        if (!_services.ContainsKey(typeof(T)))
        {
            throw new Exception($"Unable to obtain service '{typeof(T)}'");
        }

        return (T)_services[typeof(T)].Instance;
    }

    private void RegisterService(Node node)
    {
        if (_services.ContainsKey(node.GetType()))
        {
            throw new Exception($"There can only be one service of type '{node.GetType().Name}'");
        }

        ServiceAttribute serviceAttribute = node.GetType().GetCustomAttribute<ServiceAttribute>();

        if (serviceAttribute != null)
        {
            GD.Print($"Registering service: {node.GetType().Name}");
            AddService(node, serviceAttribute);
            return;
        }
    }

    /// <summary>
    /// Adds a service to the service provider.
    /// </summary>
    /// <param name="node">The node representing the service.</param>
    /// <param name="serviceAttribute">The service attribute associated with the service.</param>
    private void AddService(Node node, ServiceAttribute serviceAttribute)
    {
        Service service = new()
        {
            Instance = node,
            Persistent = serviceAttribute.IsPersistent
        };

        _services.Add(node.GetType(), service);

        RemoveServiceOnSceneChanged(service);
    }

    /// <summary>
    /// Removes a service when the scene changes, if it is not marked as persistent.
    /// </summary>
    /// <param name="service">The service to potentially remove.</param>
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

    /// <summary>
    /// Returns a string representation of the service provider, including all registered services.
    /// </summary>
    /// <returns>A formatted string of the service provider's services.</returns>
    public override string ToString()
    {
        return _services.ToFormattedString();
    }

    /// <summary>
    /// A class representing a service, including its instance and persistence status.
    /// </summary>
    public class Service
    {
        /// <summary>
        /// The instance of the service.
        /// </summary>
        public object Instance { get; set; }

        /// <summary>
        /// Indicates whether the service is persistent across scene changes.
        /// </summary>
        public bool Persistent { get; set; }
    }
}
