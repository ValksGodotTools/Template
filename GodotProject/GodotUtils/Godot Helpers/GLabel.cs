namespace GodotUtils;

using Godot;

public partial class GLabel : Label
{
    public GLabel(string text = "", int fontSize = 16)
    {
        Text = text;
        HorizontalAlignment = HorizontalAlignment.Center;
        VerticalAlignment = VerticalAlignment.Center;
        SetFontSize(fontSize);
    }

    public void SetTransparent() => SelfModulate = new Color(1, 1, 1, 0);
    public void SetFontSize(int v) => AddThemeFontSizeOverride("font_size", v);
}
