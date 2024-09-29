using Godot;
using System.Runtime.InteropServices;

namespace GodotUtils;

/// <summary>
/// This script is like the Godot OS class in a way
/// </summary>
public static class GOS
{
    // Read up on feature tags https://docs.godotengine.org/en/latest/tutorials/export/feature_tags.html
    public static bool IsExportedRelease()
    {
        return OS.HasFeature("template");
    }

    public static bool IsEditor()
    {
        return !IsExportedRelease();
    }

    public static bool IsWindows()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    }

    public static bool IsLinux()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    }

    public static bool IsMac()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
    }
}

