using Godot;
using GodotUtils;
using System;

namespace Template;

// About Scene Switching: https://docs.godotengine.org/en/latest/tutorials/scripting/singletons_autoload.html
public partial class SceneManager : Node
{
    /// <summary>
    /// The event is invoked right before the scene is changed
    /// </summary>
    public event Action<string> PreSceneChanged;

    public Node CurrentScene { get; private set; }

    private SceneTree _tree;

    public override void _Ready()
    {
        _tree = GetTree();
        Window root = _tree.Root;
        CurrentScene = root.GetChild(root.GetChildCount() - 1);
        ServiceProvider.Services.Add(this, persistent: true);

        // Gradually fade out all SFX whenever the scene is changed
        PreSceneChanged += scene =>
            ServiceProvider.Services.Get<AudioManager>().FadeOutSFX();
    }

    public void SwitchScene(Prefab prefab, TransType transType = TransType.None)
    {
        PreSceneChanged?.Invoke(prefab.ToString());

        switch (transType)
        {
            case TransType.None:
                ChangeScene(prefab, transType);
                break;
            case TransType.Fade:
                FadeTo(TransColor.Black, 2, () => ChangeScene(prefab, transType));
                break;
        }
    }

    public void SwitchScene(Scene scene, TransType transType = TransType.None)
    {
        PreSceneChanged?.Invoke(scene.ToString());

        switch (transType)
        {
            case TransType.None:
                ChangeScene(scene, transType);
                break;
            case TransType.Fade:
                FadeTo(TransColor.Black, 2, () => ChangeScene(scene, transType));
                break;
        }
    }

    /// <summary>
    /// Resets the currently active scene.
    /// </summary>
    public void ResetCurrentScene()
    {
        string sceneFilePath = _tree.CurrentScene.SceneFilePath;

        string[] words = sceneFilePath.Split("/");
        string sceneName = words[words.Length - 1].Replace(".tscn", "");

        PreSceneChanged?.Invoke(sceneName);

        // Wait for engine to be ready before switching scenes
        CallDeferred(nameof(DeferredSwitchScene), sceneFilePath, Variant.From(TransType.None));
    }

    private void ChangeScene(Scene scene, TransType transType)
    {
        // Wait for engine to be ready before switching scenes
        CallDeferred(nameof(DeferredSwitchScene), MapScenesToPaths.GetPath(scene),
            Variant.From(transType));
    }

    private void ChangeScene(Prefab prefab, TransType transType)
    {
        // Wait for engine to be ready before switching scenes
        CallDeferred(nameof(DeferredSwitchScene), MapPrefabsToPaths.GetPath(prefab),
            Variant.From(transType));
    }

    private void DeferredSwitchScene(string rawName, Variant transTypeVariant)
    {
        // Safe to remove scene now
        CurrentScene.Free();

        // Load a new scene.
        PackedScene nextScene = (PackedScene)GD.Load(rawName);

        // Instance the new scene.
        CurrentScene = nextScene.Instantiate();

        // Add it to the active scene, as child of root.
        _tree.Root.AddChild(CurrentScene);

        // Optionally, to make it compatible with the SceneTree.change_scene_to_file() API.
        _tree.CurrentScene = CurrentScene;

        TransType transType = transTypeVariant.As<TransType>();

        switch (transType)
        {
            case TransType.None:
                break;
            case TransType.Fade:
                FadeTo(TransColor.Transparent, 1);
                break;
        }
    }

    private void FadeTo(TransColor transColor, double duration, Action finished = null)
    {
        // Add canvas layer to scene
        CanvasLayer canvasLayer = new()
        {
            Layer = 10 // render on top of everything else
        };

        CurrentScene.AddChild(canvasLayer);

        // Setup color rect
        ColorRect colorRect = new()
        {
            Color = new Color(0, 0, 0, transColor == TransColor.Black ? 0 : 1),
            MouseFilter = Control.MouseFilterEnum.Ignore
        };

        // Make the color rect cover the entire screen
        colorRect.CoverEntireRect();
        canvasLayer.AddChild(colorRect);

        // Animate color rect
        new GTween(colorRect)
            .Animate(ColorRect.PropertyName.Color, new Color(0, 0, 0, transColor == TransColor.Black ? 1 : 0), duration)
            .Callback(() =>
            {
                canvasLayer.QueueFree();
                finished?.Invoke();
            });
    }

    public enum TransType
    {
        None,
        Fade
    }

    private enum TransColor
    {
        Black,
        Transparent
    }
}

