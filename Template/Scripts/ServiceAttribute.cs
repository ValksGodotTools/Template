using System;

namespace Template;

[AttributeUsage(AttributeTargets.Class)]
public class ServiceAttribute(ServiceLifeTime serviceLifeTime = ServiceLifeTime.Scene) : Attribute
{
    public ServiceLifeTime LifeTime { get; } = serviceLifeTime;

    public bool IsPersistent
    {
        get => LifeTime == ServiceLifeTime.Application;
    }
}

public enum ServiceLifeTime
{
    Application, // Persistent (Exists for the lifetime of the application)
    Scene // Non-persistent between scenes
}
