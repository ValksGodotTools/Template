namespace GodotUtils;

using Godot;

public partial class GMarginContainer : MarginContainer
{
    public GMarginContainer(int padding = 5) => SetMarginAll(5);
    public GMarginContainer(int left, int right, int top, int bottom)
    {
        SetMarginLeft(left);
        SetMarginRight(right);
        SetMarginTop(top);
        SetMarginBottom(bottom);
    }

    public void SetMarginAll(int padding)
    {
        foreach (string margin in new string[] { "left", "right", "top", "bottom" })
            AddThemeConstantOverride($"margin_{margin}", padding);
    }

    public void SetMarginLeft(int padding) =>
        AddThemeConstantOverride("margin_left", padding);

    public void SetMarginRight(int padding) =>
        AddThemeConstantOverride("margin_right", padding);

    public void SetMarginTop(int padding) =>
        AddThemeConstantOverride("margin_top", padding);

    public void SetMarginBottom(int padding) =>
        AddThemeConstantOverride("margin_bottom", padding);
}
