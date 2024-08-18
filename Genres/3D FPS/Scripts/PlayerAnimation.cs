namespace Template.FPS3D;

public partial class Player : CharacterBody3D
{
    [Export] BoneAttachment3D cameraBone;
    [Export] AnimationTree animTree;
    [Export] Node3D fpsRig;
    [Export] Node3D nodeItems;

    //bool isReloading { get => animTree.GetCondition("reload"); }

    List<Item> items = new();
    int curItemIndex;
    int nextItemIndex;

    Camera3D camera;
    Vector3 camOffset;

    bool switchingGuns = false;

    void OnReadyAnimation()
    {
        camera = GetNode<Camera3D>("%Camera3D");
        camOffset = camera.Position - Position;

        foreach (Node3D node in nodeItems.GetChildren<Node3D>())
        {
            items.Add(new Item(node));
        }

        animTree.AnimationStarted += anim =>
        {
            GD.Print(anim);

            switch (anim)
            {
                case "Holster":
                    AnimationNodeStateMachinePlayback stateMachine = animTree.GetStateMachine();
                    switchingGuns = stateMachine.GetCurrentNode() == "Holster";
                    break;
            }
        };

        animTree.AnimationFinished += async anim =>
        {
            switch (anim)
            {
                case "Rest to ADS":
                case "ADS":
                    if (!Input.IsActionPressed("ads"))
                        animTree.SetCondition("rest", true);
                    break;
                case "Reload":
                    OnFinishedReload?.Invoke();
                    break;
                case "Holster":
                    if (switchingGuns)
                    {
                        nextItemIndex = curItemIndex + 1;

                        if (nextItemIndex >= items.Count - 1)
                        {
                            curItemIndex = items.Count - 1;
                            nextItemIndex = 0;
                        }

                        animTree.AnimPlayer = items[1].AnimationPlayer.GetPath();
                        cameraBone.SetExternalSkeleton(items[1].SkeletonRig.GetPath());
                        items[0].SetVisible(false);

                        // If we do not wait for one frame then the wrong animation will play
                        // in the next frame creating a sort of visual glitch. This appears to
                        // be a bug with the Godot engine.
                        await this.WaitOneFrame();

                        items[1].SetVisible(true);

                        AnimationNodeStateMachinePlayback stateMachine = animTree.GetStateMachine();
                        stateMachine.Start("Draw");
                    }
                    break;
            }
        };
    }

    void OnPhysicsProcessAnimation()
    {
        // Mouse motion
        Quaternion camTarget = Quaternion.FromEuler(cameraTarget);

        camera.Position = Position + camOffset;
        camera.Quaternion = (camTarget * GetAnimationRotations()).Normalized();

        fpsRig.Position = camera.Position;
        fpsRig.Quaternion = camTarget;

        if (Input.IsActionJustPressed("reload"))
        {
            animTree.SetCondition("reload", true);
        }

        if (Input.IsActionJustPressed("ads"))
        {
            animTree.SetCondition("ads", true);
        }

        if (Input.IsActionJustReleased("ads"))
        {
            animTree.SetCondition("rest", true);
        }

        if (Input.IsActionJustPressed("inspect"))
        {
            animTree.SetCondition("inspect", true);
        }
    }

    Quaternion GetAnimationRotations()
    {
        // The camera bone
        Quaternion camBoneQuat = new Quaternion(cameraBone.Basis);

        // Account for annoying offset from the camera bone
        Quaternion offset = Quaternion.FromEuler(new Vector3(-Mathf.Pi / 2, -Mathf.Pi, 0));

        // The end result (multiplying order matters and always normalize to prevent errors)
        return (camBoneQuat * offset).Normalized();
    }
}

public class Item // An item the player can hold
{
    public Skeleton3D SkeletonRig { get; set; }
    public AnimationPlayer AnimationPlayer { get; set; }

    Node3D parent;

    public Item(Node3D parent)
    {
        this.parent = parent;
        SkeletonRig = parent.GetNode<Skeleton3D>("Armature/Skeleton3D");
        AnimationPlayer = parent.GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public void SetVisible(bool v) => parent.Visible = v;
}
