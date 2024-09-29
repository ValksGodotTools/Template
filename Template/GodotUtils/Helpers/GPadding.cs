using Godot;

namespace GodotUtils;

public partial class GPadding : Control
{
    public GPadding(int paddingX = 0, int paddingY = 0)
    {
        CustomMinimumSize = new Vector2(paddingX, paddingY);
    }
}

