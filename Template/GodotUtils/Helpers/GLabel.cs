using Godot;

namespace GodotUtils;

public partial class GLabel : Label
{
    public GLabel(string text = "", int fontSize = 16)
    {
        Text = text;
        HorizontalAlignment = HorizontalAlignment.Center;
        VerticalAlignment = VerticalAlignment.Center;
        SetFontSize(fontSize);
    }

    public GLabel SetTransparent()
    {
        SelfModulate = new Color(1, 1, 1, 0);
        return this;
    }

    public GLabel SetFontSize(int v)
    {
        AddThemeFontSizeOverride("font_size", v);
        return this;
    }
}

