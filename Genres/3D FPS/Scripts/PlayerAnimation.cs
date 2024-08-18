namespace Template.FPS3D;

public partial class Player : CharacterBody3D
{
    [Export] BoneAttachment3D cameraBone;
    [Export] AnimationTree animTree;
    [Export] Node3D fpsRig;

    //bool isReloading { get => animTree.GetCondition("reload"); }

    void OnReadyAnimation()
    {
        animTree.AnimationFinished += anim =>
        {
            if (anim == "Rest to ADS" && !Input.IsActionPressed("ads"))
            {
                animTree.SetCondition("rest", true);
            }

            if (anim == "ADS" && !Input.IsActionPressed("ads"))
            {
                animTree.SetCondition("rest", true);
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
