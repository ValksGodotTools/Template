using Godot;
using GodotUtils;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace Template;

public partial class ServiceProvider : Node
{
    public static ServiceProvider Services { get; private set; }
    
    private Dictionary<Type, Service> _services = [];

    public override void _EnterTree()
    {
        Services = this;
        RegisterServices();
    }

    public T Get<T>()
    {
        if (!_services.ContainsKey(typeof(T)))
        {
            throw new Exception($"Unable to obtain service '{typeof(T)}'");
        }

        return (T)_services[typeof(T)].Instance;
    }

    private void RegisterServices()
    {
        IEnumerable<Node> scriptNodes = GetScriptNodes();
        Dictionary<Type, ServiceAttribute> cachedAttributes = CacheServiceAttributes();

        foreach (Node node in scriptNodes)
        {
            foreach (KeyValuePair<Type, ServiceAttribute> kvp in cachedAttributes)
            {
                Type type = kvp.Key;
                ServiceAttribute serviceAttribute = kvp.Value;

                if (type.IsAssignableTo(node.GetType()))
                {
                    AddService(node, serviceAttribute);
                    break;
                }
            }
        }
    }

    private IEnumerable<Node> GetScriptNodes()
    {
        return GetTree().Root.GetChildren<Node>().Where(x => x.GetScript().VariantType != Variant.Type.Nil);
    }

    private Dictionary<Type, ServiceAttribute> CacheServiceAttributes()
    {
        Dictionary<Type, ServiceAttribute> cachedAttributes = [];

        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
        {
            ServiceAttribute serviceAttribute = (ServiceAttribute)type.GetCustomAttribute(typeof(ServiceAttribute));

            if (serviceAttribute != null && type.IsAssignableTo(typeof(Node)))
            {
                cachedAttributes[type] = serviceAttribute;
            }
        }

        return cachedAttributes;
    }

    private void AddService(Node node, ServiceAttribute serviceAttribute)
    {
        GD.Print("Added " + node.Name);

        Service service = new()
        {
            Instance = node,
            Persistent = serviceAttribute.Persistent
        };

        _services.Add(node.GetType(), service);

        RemoveServiceOnSceneChanged(service);
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

    public override string ToString()
    {
        return _services.ToFormattedString();
    }

    public class Service
    {
        public object Instance { get; set; }
        public bool Persistent { get; set; }
    }
}
