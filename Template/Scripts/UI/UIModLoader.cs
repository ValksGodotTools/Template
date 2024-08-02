using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;
using System.Xml.Linq;

namespace Template;

public partial class UIModLoader : Node
{
    Label uiName;
    Label uiModVersion;
    Label uiGameVersion;
    Label uiDependencies;
    Label uiDescription;
    Label uiAuthors;
    Label uiIncompatibilities;

    public override void _Ready()
    {
        Node uiMods = GetNode("%Mods");
        
        uiName = GetNode<Label>("%Name");
        uiModVersion = GetNode<Label>("%ModVersion");
        uiGameVersion = GetNode<Label>("%GameVersion");
        uiDependencies = GetNode<Label>("%Dependencies");
        uiDescription = GetNode<Label>("%Description");
        uiAuthors = GetNode<Label>("%Authors");
        uiIncompatibilities = GetNode<Label>("%Incompatibilities");

        Dictionary<string, ModInfo> mods = Global.Services.Get<ModLoader>().Mods;

        bool first = true;

        foreach (ModInfo modInfo in mods.Values)
        {
            Button btn = new Button();
            btn.ToggleMode = true;
            btn.Text = modInfo.Name;
            btn.Pressed += () =>
            {
                DisplayModInfo(modInfo);
            };

            uiMods.AddChild(btn);

            if (first)
            {
                first = false;
                btn.GrabFocus();
                DisplayModInfo(modInfo);
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            Global.Services.Get<SceneManager>().SwitchScene("UI/main_menu");
        }
    }

    void DisplayModInfo(ModInfo modInfo)
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

        uiAuthors.Text = modInfo.Author;
    }

    async void _on_restart_game_pressed()
    {
        //OS.CreateProcess(OS.GetExecutablePath(), null);
        OS.CreateInstance(null);
        await GetNode<Global>("/root/Global").QuitAndCleanup();
    }

    void _on_open_mods_folder_pressed()
    {
        Process.Start(new ProcessStartInfo(@$"{ProjectSettings.GlobalizePath("res://Mods")}") { UseShellExecute = true });
    }
}
