using Godot;
using BlendMode = Godot.CanvasItemMaterial.BlendModeEnum;
using LightMode = Godot.CanvasItemMaterial.LightModeEnum;

namespace Template.TopDown2D;

[GlobalClass]
public partial class EntityConfig : Resource
{
    [Export] public Color Color { get; set; } = Colors.White;
    [Export] public BlendMode BlendMode { get; set; } = BlendMode.Mix;
    [Export] public LightMode LightMode { get; set; } = LightMode.Normal;
}
