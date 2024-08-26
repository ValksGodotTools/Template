namespace Template;

// About Scene Switching: https://docs.godotengine.org/en/latest/tutorials/scripting/singletons_autoload.html
public partial class SceneManager : Node
{
    /// <summary>
    /// The event is invoked right before the scene is changed
    /// </summary>
    public event Action<string> PreSceneChanged;

    public Node CurrentScene { get; private set; }

    SceneTree tree;

    public override void _Ready()
    {
        tree = GetTree();
        Window root = tree.Root;
        CurrentScene = root.GetChild(root.GetChildCount() - 1);
        Global.Services.Add(this, persistent: true);

        // Gradually fade out all SFX whenever the scene is changed
        PreSceneChanged += name =>
            Global.Services.Get<AudioManager>().FadeOutSFX();
    }

    /// <summary>
    /// Scenes are loaded from the 'res://Scenes/' directory. For example a name with 
    /// "level_1" would be 'res://Scenes/level_1.tscn'
    /// </summary>
    public void SwitchScene(string name, TransType transType = TransType.None)
    {
        PreSceneChanged?.Invoke(name);

        switch (transType)
        {
            case TransType.None:
                ChangeScene(name, transType);
                break;
            case TransType.Fade:
                FadeTo(TransColor.Black, 2, () => ChangeScene(name, transType));
                break;
        }
    }

    /// <summary>
    /// Resets the currently active scene.
    /// </summary>
    public void ResetCurrentScene()
    {
        string sceneFilePath = tree.CurrentScene.SceneFilePath;

        string[] words = sceneFilePath.Split("/");
        string sceneName = words[words.Length - 1].Replace(".tscn", "");

        PreSceneChanged?.Invoke(sceneName);

        // Wait for engine to be ready before switching scenes
        CallDeferred(nameof(DeferredSwitchScene), sceneFilePath, Variant.From(TransType.None));
    }

    void ChangeScene(string name, TransType transType)
    {
        // Wait for engine to be ready before switching scenes
        CallDeferred(nameof(DeferredSwitchScene), $"res://Scenes/{name}.tscn",
            Variant.From(transType));
    }

    void DeferredSwitchScene(string rawName, Variant transTypeVariant)
    {
        // Safe to remove scene now
        CurrentScene.Free();

        // Load a new scene.
        PackedScene nextScene = (PackedScene)GD.Load(rawName);

        // Instance the new scene.
        CurrentScene = nextScene.Instantiate();

        // Add it to the active scene, as child of root.
        tree.Root.AddChild(CurrentScene);

        // Optionally, to make it compatible with the SceneTree.change_scene_to_file() API.
        tree.CurrentScene = CurrentScene;

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

    void FadeTo(TransColor transColor, double duration, Action finished = null)
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
        GTween tween = new(colorRect);
        tween.Animate(ColorRect.PropertyName.Color, new Color(0, 0, 0, transColor == TransColor.Black ? 1 : 0), duration);
        tween.Callback(() =>
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

    enum TransColor
    {
        Black,
        Transparent
    }
}
