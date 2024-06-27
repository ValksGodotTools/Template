using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;

namespace Template;

public partial class UIModLoader : Node
{
    RichTextLabel console;
    Node uiMods;

    Label uiName;
    Label uiModVersion;
    Label uiGameVersion;
    Label uiDependencies;
    Label uiDescription;
    Label uiAuthors;
    Label uiIncompatibilities;

    public override void _Ready()
    {
        console = GetNode<RichTextLabel>("%Console");
        uiMods = GetNode("%Mods");
        uiName = GetNode<Label>("%Name");
        uiModVersion = GetNode<Label>("%ModVersion");
        uiGameVersion = GetNode<Label>("%GameVersion");
        uiDependencies = GetNode<Label>("%Dependencies");
        uiDescription = GetNode<Label>("%Description");
        uiAuthors = GetNode<Label>("%Authors");
        uiIncompatibilities = GetNode<Label>("%Incompatibilities");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            Global.Services.Get<SceneManager>().SwitchScene("UI/main_menu");
        }
    }

    void _on_load_mods_pressed()
    {
        uiMods.QueueFreeChildren();
        console.Clear();

        string modsPath = ProjectSettings.GlobalizePath("res://Mods");

        // Ensure "Mods" directory always exists
        Directory.CreateDirectory(modsPath);

        using DirAccess dir = DirAccess.Open(modsPath);

        if (dir == null)
        {
            Log("Failed to open Mods directory has it does not exist");
            return;
        }

        dir.ListDirBegin();

        string filename = dir.GetNext();

        while (filename != "")
        {
            if (dir.CurrentIsDir())
            {
                string modRoot = $@"{modsPath}/{filename}";
                string modJson = $@"{modRoot}/mod.json";

                if (File.Exists(modJson))
                {
                    string jsonFileContents = File.ReadAllText(modJson);

                    jsonFileContents = jsonFileContents.Replace("*", "Any");

                    ModInfo modInfo = JsonSerializer.Deserialize<ModInfo>(jsonFileContents, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

                    Button btn = new Button();
                    btn.ToggleMode = true;
                    btn.Text = modInfo.Name;
                    btn.Pressed += () =>
                    {
                        uiName.Text = modInfo.Name;
                        uiModVersion.Text = modInfo.ModVersion;
                        uiGameVersion.Text = modInfo.GameVersion;

                        if (modInfo.Dependencies.Count != 0)
                        {
                            uiDependencies.Text = modInfo.Dependencies.Print();
                        }
                        else
                        {
                            uiDependencies.Text = "None";
                        }
                        
                        if (modInfo.Incompatibilities.Count != 0)
                        {
                            uiIncompatibilities.Text = modInfo.Incompatibilities.Print();
                        }
                        else
                        {
                            uiIncompatibilities.Text = "None";
                        }
                        
                        if (!string.IsNullOrWhiteSpace(modInfo.Description))
                        {
                            uiDescription.Text = modInfo.Description;
                        }
                        else
                        {
                            uiDescription.Text = "The author did not set a description for this mod";
                        }
                        
                        uiAuthors.Text = modInfo.Authors.Print();
                    };
                    uiMods.AddChild(btn);

                    // Load dll
                    string dllPath = $@"{modRoot}/Mod.dll";

                    if (File.Exists(dllPath))
                    {
                        byte[] file = File.ReadAllBytes(dllPath);
                        AssemblyLoadContext context = AssemblyLoadContext.GetLoadContext(typeof(Godot.Bridge.ScriptManagerBridge).Assembly);
                        Assembly assembly = context.LoadFromStream(new MemoryStream(file));
                        Godot.Bridge.ScriptManagerBridge.LookupScriptsInAssembly(assembly);
                    }

                    // Load pck
                    string pckPath = $@"{modRoot}/mod.pck";

                    if (File.Exists(pckPath))
                    {
                        bool success = ProjectSettings.LoadResourcePack(pckPath, false);

                        if (success)
                        {
                            // TODO: Change "res://mod.tscn" to "res://mod_id/mod.tscn"
                            PackedScene importedScene = (PackedScene)ResourceLoader.Load("res://mod.tscn");
                            Node mod = importedScene.Instantiate<Node>();
                            GetTree().Root.AddChild(mod);
                        }
                        else
                        {
                            Log($"Failed to load pck file for mod '{modInfo.Name}'");
                        }
                    }
                }
                else
                {
                    Log($"The mod folder '{filename}' does not have a mod.json so it will not be loaded");
                }
            }

            filename = dir.GetNext();
        }
    }

    void Log(object obj)
    {
        console.AddText($"{obj}\n");
    }
}

public class ModInfo
{
    public string Name { get; set; }
    public string Id { get; set; }
    public string ModVersion { get; set; }
    public string GameVersion { get; set; }
    public string Description { get; set; }
    public string[] Authors { get; set; }
    public Dictionary<string, string> Dependencies { get; set; }
    public Dictionary<string, string> Incompatibilities { get; set; }
}
