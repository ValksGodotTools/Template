using Godot;
using GodotUtils;
using System.Collections.Generic;

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

        RecreateCameraBone(curItemIndex);

        animTree.AnimationStarted += anim =>
        {
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
                        int nextItemIndex = (curItemIndex + 1) % items.Count;

                        animTree.AnimPlayer = items[nextItemIndex].AnimationPlayer.GetPath();
                        
                        items[curItemIndex].SetVisible(false);
                        
                        // If we do not wait for one frame then the wrong animation will play
                        // in the next frame creating a sort of visual glitch. This appears to
                        // be a bug with the Godot engine.
                        await this.WaitOneFrame();

                        RecreateCameraBone(nextItemIndex);
                        items[nextItemIndex].SetVisible(true);

                        AnimationNodeStateMachinePlayback stateMachine = animTree.GetStateMachine();
                        stateMachine.Start("Draw");

                        curItemIndex = (curItemIndex + 1) % items.Count;
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
        Quaternion camBoneQuat = new(cameraBone.Basis);

        // Account for annoying offset from the camera bone
        Quaternion offset = Quaternion.FromEuler(new Vector3(-Mathf.Pi / 2, -Mathf.Pi, 0));

        // The end result (multiplying order matters and always normalize to prevent errors)
        return (camBoneQuat * offset).Normalized();
    }

    // We have to re-create the camera bone everytime because if we do not, for some reason
    // Godot has decided to output an error if we try to set the external skeleton to
    // another skeleton
    void RecreateCameraBone(int itemIndex)
    {
        if (GodotObject.IsInstanceValid(cameraBone))
            cameraBone.QueueFree();

        cameraBone = new();
        fpsRig.AddChild(cameraBone);
        cameraBone.BoneName = "Camera";

        cameraBone.SetExternalSkeleton(items[itemIndex].SkeletonRig.GetPath());

        // Will output an error if this is the same camera bone so that's why we re-create
        // it every time
        cameraBone.SetUseExternalSkeleton(true);
    }
}

