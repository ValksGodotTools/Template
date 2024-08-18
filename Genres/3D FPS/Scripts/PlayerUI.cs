namespace Template.FPS3D;

public partial class Player : CharacterBody3D
{
    [Export] OptionsManager options;

    Vector3 cameraTarget;
    Vector2 mouseInput;
    float mouseSensitivity;

    void OnReadyUI()
    {
        mouseSensitivity = options.Options.MouseSensitivity * 0.0001f;

        UIOptionsGameplay gameplay = GetNode<UIPopupMenu>("%PopupMenu")
            .Options.GetNode<UIOptionsGameplay>("%Gameplay");

        gameplay.OnMouseSensitivityChanged += value =>
        {
            mouseSensitivity = value * 0.0001f;
        };
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.MouseMode != Input.MouseModeEnum.Captured)
            return;

        if (@event is InputEventMouseMotion motion)
        {
            mouseInput = motion.Relative;

            cameraTarget += new Vector3(
                -motion.Relative.Y * mouseSensitivity,
                -motion.Relative.X * mouseSensitivity, 0);

            // Prevent camera from looking too far up or down
            Vector3 rotDeg = cameraTarget;
            rotDeg.X = Mathf.Clamp(rotDeg.X, -89f.ToRadians(), 89f.ToRadians());
            cameraTarget = rotDeg;
        }
    }
}
