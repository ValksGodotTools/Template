using Godot;
using GodotUtils;

namespace Template.FPS3D;

public partial class Player : CharacterBody3D
{
    [Export] private OptionsManager options;
    private Vector3 _cameraTarget;
    private Vector2 _mouseInput;
    private float _mouseSensitivity;

    private void OnReadyUI()
    {
        _mouseSensitivity = options.Options.MouseSensitivity * 0.0001f;

        UIOptionsGameplay gameplay = GetNode<UIPopupMenu>("%PopupMenu")
            .Options.GetNode<UIOptionsGameplay>("%Gameplay");

        gameplay.OnMouseSensitivityChanged += value =>
        {
            _mouseSensitivity = value * 0.0001f;
        };
    }

    private void OnPhysicsProcessUI()
    {
        if (Input.IsActionJustPressed("next_held_item"))
        {
            animTree.SetCondition("holster", true);
        }

        if (Input.IsActionJustPressed("previous_held_item"))
        {
            //animTree.SetCondition("holster", true);
        }
    }

    private void OnInputUI(InputEvent @event)
    {
        if (Input.MouseMode != Input.MouseModeEnum.Captured)
            return;

        if (@event is InputEventMouseMotion motion)
        {
            _mouseInput = motion.Relative;

            _cameraTarget += new Vector3(
                -motion.Relative.Y * _mouseSensitivity,
                -motion.Relative.X * _mouseSensitivity, 0);

            // Prevent camera from looking too far up or down
            Vector3 rotDeg = _cameraTarget;
            rotDeg.X = Mathf.Clamp(rotDeg.X, -89f.ToRadians(), 89f.ToRadians());
            _cameraTarget = rotDeg;
        }
    }
}

