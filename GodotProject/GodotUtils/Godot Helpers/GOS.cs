namespace GodotUtils;

using Godot;
using System.Runtime.InteropServices;

/// <summary>
/// This script is like the Godot OS class in a way
/// </summary>
public static class GOS
{
    // Read up on feature tags https://docs.godotengine.org/en/latest/tutorials/export/feature_tags.html
    public static bool IsExportedRelease() => OS.HasFeature("template");

    public static bool IsEditor() => !IsExportedRelease();

    public static bool IsWindows() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    public static bool IsLinux() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

    public static bool IsMac() => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
}
