using Godot;
using System;

namespace GodotUtils;

/// <summary>
/// A custom wrapper for Godot's ColorPickerButton with additional functionality.
/// </summary>
public class GColorPickerButton
{
    /// <summary>
    /// Event triggered when the color is changed.
    /// </summary>
    public event Action<Color> OnColorChanged;

    /// <summary>
    /// The underlying ColorPickerButton control.
    /// </summary>
    public ColorPickerButton Control { get; }

    /// <summary>
    /// Initializes a new instance of the GColorPickerButton class with the default color (black).
    /// </summary>
    public GColorPickerButton() : this(Colors.Black)
    {

    }

    /// <summary>
    /// Initializes a new instance of the GColorPickerButton class with the specified initial color.
    /// </summary>
    /// <param name="initialColor">The initial color of the ColorPickerButton.</param>
    public GColorPickerButton(Color initialColor)
    {
        Control = new()
        {
            CustomMinimumSize = Vector2.One * 30
        };

        Control.ColorChanged += color =>
        {
            OnColorChanged?.Invoke(color);
        };

        Control.PickerCreated += () =>
        {
            ColorPicker picker = Control.GetPicker();

            picker.Color = initialColor;

            PopupPanel popupPanel = picker.GetParent<PopupPanel>();

            popupPanel.InitialPosition = Window.WindowInitialPosition.Absolute;

            popupPanel.AboutToPopup += () =>
            {
                Vector2 viewportSize = popupPanel.GetTree().Root.GetViewport().GetVisibleRect().Size;

                // Position the ColorPicker to be at the top right of the screen
                popupPanel.Position = new Vector2I(
                    (int)(viewportSize.X - popupPanel.Size.X),
                    0);
            };
        };

        Control.PopupClosed += Control.ReleaseFocus;
    }
}
