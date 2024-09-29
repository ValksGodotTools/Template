using Godot;
using System;

namespace GodotUtils;

/// <summary>
/// Represents a custom OptionButton that allows adding items and handling item selection events.
/// </summary>
public class GOptionButton : GOptionButtonBase
{
    /// <summary>
    /// Event triggered when an item is selected from the OptionButton.
    /// </summary>
    public event Action<int> OnItemSelected;

    /// <summary>
    /// Initializes a new instance of the <see cref="GOptionButton"/> class with the specified items.
    /// </summary>
    /// <param name="items">The items to add to the OptionButton.</param>
    public GOptionButton(params string[] items) : base()
    {
        foreach (string item in items)
        {
            Control.AddItem(item);
        }

        Control.ItemSelected += item =>
        {
            OnItemSelected?.Invoke((int)item);
            Control.ReleaseFocus();
        };
    }

    /// <summary>
    /// Selects an item in the OptionButton by its index.
    /// </summary>
    /// <param name="index">The index of the item to select.</param>
    public void Select(int index)
    {
        Control.Select(index);
    }
}

/// <summary>
/// Represents a custom OptionButton that binds to an enum type and handles enum value selection events.
/// </summary>
public class GOptionButtonEnum : GOptionButtonBase
{
    /// <summary>
    /// Event triggered when an enum value is selected from the OptionButton. The selected enum object is returned.
    /// </summary>
    public event Action<object> OnItemSelected;

    private Type _enumType;

    /// <summary>
    /// Initializes a new instance of the <see cref="GOptionButtonEnum"/> class with the specified enum type.
    /// </summary>
    /// <param name="enumType">The enum type to bind to the OptionButton.</param>
    /// <exception cref="ArgumentException">Thrown if the provided type is not an enum.</exception>
    public GOptionButtonEnum(Type enumType) : base()
    {
        if (!enumType.IsEnum)
        {
            throw new ArgumentException("enumType must be a enum type");
        }

        _enumType = enumType;

        foreach (object item in Enum.GetValues(enumType))
        {
            Control.AddItem(item.ToString().AddSpaceBeforeEachCapital());
        }

        Control.ItemSelected += item =>
        {
            object selectedValue = Enum.GetValues(enumType).GetValue(item);
            OnItemSelected?.Invoke(selectedValue);
            Control.ReleaseFocus();
        };
    }

    /// <summary>
    /// Selects an enum value in the OptionButton.
    /// </summary>
    /// <param name="initialValue">The enum value to select.</param>
    public void Select(object initialValue)
    {
        int selectedIndex = Array.IndexOf(Enum.GetValues(_enumType), initialValue);
        Control.Select(selectedIndex);
    }
}

/// <summary>
/// Base class for custom OptionButton implementations.
/// </summary>
public abstract class GOptionButtonBase
{
    /// <summary>
    /// Gets the underlying OptionButton control.
    /// </summary>
    public OptionButton Control { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GOptionButtonBase"/> class.
    /// </summary>
    public GOptionButtonBase()
    {
        Control = new OptionButton
        {
            Alignment = HorizontalAlignment.Center
        };
    }
}