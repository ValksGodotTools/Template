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

    List<Item> _items = [];
    int _curItemIndex;

    Camera3D _camera;
    Vector3 _camOffset;

    bool _switchingGuns;

    void OnReadyAnimation()
    {
        _camera = GetNode<Camera3D>("%Camera3D");
        _camOffset = _camera.Position - Position;

        foreach (Node3D node in nodeItems.GetChildren(false))
        {
            _items.Add(new Item(node));
        }

        RecreateCameraBone(_curItemIndex);

        animTree.AnimationStarted += anim =>
        {
            switch (anim)
            {
                case "Holster":
                    AnimationNodeStateMachinePlayback stateMachine = animTree.GetStateMachine();
                    _switchingGuns = stateMachine.GetCurrentNode() == "Holster";
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
                    if (_switchingGuns)
                    {
                        int nextItemIndex = (_curItemIndex + 1) % _items.Count;

                        animTree.AnimPlayer = _items[nextItemIndex].AnimationPlayer.GetPath();
                        
                        _items[_curItemIndex].SetVisible(false);
                        
                        // If we do not wait for one frame then the wrong animation will play
                        // in the next frame creating a sort of visual glitch. This appears to
                        // be a bug with the Godot engine.
                        await this.WaitOneFrame();

                        RecreateCameraBone(nextItemIndex);
                        _items[nextItemIndex].SetVisible(true);

                        AnimationNodeStateMachinePlayback stateMachine = animTree.GetStateMachine();
                        stateMachine.Start("Draw");

                        _curItemIndex = (_curItemIndex + 1) % _items.Count;
                    }
                    break;
            }
        };
    }

    void OnPhysicsProcessAnimation()
    {
        // Mouse motion
        Quaternion camTarget = Quaternion.FromEuler(_cameraTarget);

        _camera.Position = Position + _camOffset;
        _camera.Quaternion = (camTarget * GetAnimationRotations()).Normalized();

        fpsRig.Position = _camera.Position;
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

        cameraBone.SetExternalSkeleton(_items[itemIndex].SkeletonRig.GetPath());

        // Will output an error if this is the same camera bone so that's why we re-create
        // it every time
        cameraBone.SetUseExternalSkeleton(true);
    }
}

