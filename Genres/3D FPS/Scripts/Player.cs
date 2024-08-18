namespace Template.FPS3D;

public partial class Player : CharacterBody3D
{   
    event Action OnFinishedReload;

    public override void _Ready()
    {
        OnReadyUI();
        OnReadyAnimation();
    }

    public override void _PhysicsProcess(double delta)
    {
        OnPhysicsProcessMotion(delta);
        OnPhysicsProcessAnimation();
    }
}
