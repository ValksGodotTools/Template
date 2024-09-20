using System;

namespace Template;

[AttributeUsage(AttributeTargets.Class)]
public class ServiceAttribute(bool persistent = false) : Attribute
{
    public bool Persistent { get; private set; } = persistent;
}
