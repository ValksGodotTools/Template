namespace Template;

// About Scene Switching: https://docs.godotengine.org/en/latest/tutorials/scripting/singletons_autoload.html
// Members of this class were set to static because this will exist for the
// duration of the applications life and there should be no issues with
// using these functions anywhere at anytime.
public partial class SceneManager : Node
{
    public event Action<string> SceneChanged;

    public Node CurrentScene { get; set; }

    SceneTree tree;

    /// <summary>
    /// Scenes are loaded from the 'res://Scenes/' directory. For example a name with 
    /// "level_1" would be 'res://Scenes/level_1.tscn'
    /// </summary>
    public void SwitchScene(string name, TransType transType = TransType.None)
    {
        SceneChanged?.Invoke(name);

        switch (transType)
        {
            case TransType.None:
                ChangeScene(transType);
                break;
            case TransType.Fade:
                FadeTo(TransColor.Black, 2, () => ChangeScene(transType));
                break;
        }

        void ChangeScene(TransType transType)
        {
            // Wait for engine to be ready to switch scene
            CallDeferred(nameof(DeferredSwitchScene), name,
                Variant.From(transType));
        }
    }

    public override void _Ready()
    {
        tree = GetTree();
        Window root = tree.Root;
        CurrentScene = root.GetChild(root.GetChildCount() - 1);
        Global.Services.Add(this);

        // Gradually fade out all SFX whenever the scene is changed
        SceneChanged += name =>
            Global.Services.Get<AudioManager>().FadeOutSFX();
    }

    void DeferredSwitchScene(string name, Variant transTypeVariant)
    {
        // Safe to remove scene now
        CurrentScene.Free();

        // Load a new scene.
        var nextScene = (PackedScene)GD.Load($"res://Scenes/{name}.tscn");

        // Instance the new scene.
        CurrentScene = nextScene.Instantiate();

        // Add it to the active scene, as child of root.
        tree.Root.AddChild(CurrentScene);

        // Optionally, to make it compatible with the SceneTree.change_scene_to_file() API.
        tree.CurrentScene = CurrentScene;

        var transType = transTypeVariant.As<TransType>();

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
        var canvasLayer = new CanvasLayer
        {
            Layer = 10 // render on top of everything else
        };

        CurrentScene.AddChild(canvasLayer);

        // Setup color rect
        var colorRect = new ColorRect
        {
            Color = new Color(0, 0, 0, transColor == TransColor.Black ? 0 : 1),
            MouseFilter = Control.MouseFilterEnum.Ignore
        };

        // Make the color rect cover the entire screen
        colorRect.CoverEntireRect();
        canvasLayer.AddChild(colorRect);

        // Animate color rect
        var tween = new GTween(colorRect);
        tween.AnimateColor(new Color(0, 0, 0, transColor == TransColor.Black ? 1 : 0), duration);
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
