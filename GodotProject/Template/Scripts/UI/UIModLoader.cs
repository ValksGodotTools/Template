using Godot;
using GodotUtils;
using System.Collections.Generic;
using System.Diagnostics;

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
            Button btn = new();
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
            Game.SwitchScene(Scene.UIMainMenu);
        }
    }

    void DisplayModInfo(ModInfo modInfo)
    {
        uiName.Text = modInfo.Name;
        uiModVersion.Text = modInfo.ModVersion;
        uiGameVersion.Text = modInfo.GameVersion;

        uiDependencies.Text = modInfo.Dependencies.Count != 0 ? 
            modInfo.Dependencies.Print() : "None";

        uiIncompatibilities.Text = modInfo.Incompatibilities.Count != 0 ? 
            modInfo.Incompatibilities.Print() : "None";

        uiDescription.Text = !string.IsNullOrWhiteSpace(modInfo.Description) ? 
            modInfo.Description : "The author did not set a description for this mod";

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

