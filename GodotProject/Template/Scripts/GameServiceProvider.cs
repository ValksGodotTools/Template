namespace Template;

public class GameServiceProvider : ServiceProvider
{
    /// <summary>
    /// Add a instance to be tracked as a service. If the service is marked
    /// as persistent it will not get cleaned up on scene change.
    /// </summary>
    public override Service Add(object instance, bool persistent = false)
    {
        Service service = base.Add(instance, persistent);
        RemoveServiceOnSceneChanged(service);
        return service;
    }

    void RemoveServiceOnSceneChanged(Service service)
    {
        // Do not remove persistent services
        // Only remove services if the SceneManager service exists in services
        if (service.Persistent || !services.ContainsKey(typeof(SceneManager)))
            return;

        // The scene has changed, remove all non-persistent services
        SceneManager sceneManager = (SceneManager)services[typeof(SceneManager)].Instance;
        sceneManager.PreSceneChanged += Cleanup;

        void Cleanup(string sceneName)
        {
            // Stop listening to PreSceneChanged
            sceneManager.PreSceneChanged -= Cleanup;

            // Remove the service
            //GD.Print($"Cleaned up service '{service.Instance.GetType().Name}'");
            services.Remove(service.Instance.GetType());
        }
    }
}
