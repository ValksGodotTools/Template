using Godot;

namespace Visualize.Core;

#if TOOLS
[Tool]
public partial class Visualize : EditorPlugin
{
    private const string PLUGIN = "Visualize";
    private const string AUTOLOAD = "VisualizeAutoload";

    public override void _EnablePlugin()
    {
        CSharpScript autoloadScript = GD.Load<CSharpScript>($"res://addons/{PLUGIN.ToLower()}/Scripts/{AUTOLOAD}.cs");

        if (!ProjectSettings.HasSetting("autoload/" + AUTOLOAD))
        {
            ProjectSettings.SetSetting("autoload/" + AUTOLOAD, autoloadScript.ResourcePath);
            Error error = ProjectSettings.Save();
            GD.Print("Enabled plugin");
            if (error != Error.Ok)
            {
                GD.PushWarning($"Failed to save project settings when entering {PLUGIN} plugin");
            }
        }
    }

    public override void _DisablePlugin()
    {
        if (ProjectSettings.HasSetting("autoload/" + AUTOLOAD))
        {
            ProjectSettings.Clear("autoload/" + AUTOLOAD);
            Error error = ProjectSettings.Save();
            GD.Print("Disabled plugin");
            if (error != Error.Ok)
            {
                GD.PushWarning($"Failed to save project settings when exiting {PLUGIN} plugin");
            }
        }
    }
}
#endif
